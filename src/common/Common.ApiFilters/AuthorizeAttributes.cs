using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

using Extensions.HttpActionContextExtension;
using inflector_extension;
using System.Web;
using System.Security.Principal;

using Extensions.ClaimExtensions;
using Common.ApiClaims;

namespace Common.ApiFilters
{
    
    public class SimpleRoleAuthorizeFilter : AuthorizationFilterAttribute
    {
        public List<string> Roles { get; set; }

        public SimpleRoleAuthorizeFilter(params string[] roles)
        {
			Roles = new List<string>();

			if (roles != null)
				Roles.AddRange(roles);
        }

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (actionContext.SkipAuthorization())
                return;

            var claimsPrincipal = actionContext.RequestContext.Principal as ClaimsPrincipal;

            if (claimsPrincipal.Identity.IsAuthenticated && (IsInRole(claimsPrincipal)))
                return;

            actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
        }

        private bool IsInRole(ClaimsPrincipal claimsPrincipal)
        {
            SimpleRoleClaim item = claimsPrincipal.GetClaim<SimpleRoleClaim>("projectRequestRole");

            /*item is null when domain user makes a request without the bearer token. Which means that they are trying to hit the endpoint through a browser or that they haven't called /token first*/
            if (item == null || !Roles.Any(i => i.Trim().ToLower().Equals(item.Role.Trim().ToLower())))
                return false;

            return true;
        }
    }

    public class RoleAuthorizeFilter : AuthorizationFilterAttribute
    {
        public string Organization { get; set; }
        public List<string> GlobalRoles { get; set; }
        public List<string> OrgRoles { get; set; }
        public List<string> AppRoles { get; set; }

        public string App { get; set; }

        public RoleAuthorizeFilter(string organization = "", 
                                                string[] globalRoles = null,
                                                string[] orgRoles = null,
                                                string app = "",
                                                string[] appRoles = null)
        {
            globalRoles = globalRoles ?? new string[0];
            orgRoles = orgRoles ?? new string[0];
            appRoles = appRoles ?? new string[0];
            

            Organization = organization.ToLower();
            OrgRoles = orgRoles.Select(i => i.ToLower()).ToList();
            GlobalRoles = globalRoles.Select(i => i.ToLower()).ToList();
            AppRoles = appRoles.Select(i => i.ToLower()).ToList();
            App = app;
        }

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (actionContext.SkipAuthorization())
                return;

            var claimsPrincipal = actionContext.RequestContext.Principal as ClaimsPrincipal;

            if (claimsPrincipal.Identity.IsAuthenticated && (IsInGlobalRole(claimsPrincipal) || IsInOrgRole(claimsPrincipal) || IsInAppRole(claimsPrincipal)))
                return;

            actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
        }

        private bool IsInAppRole(ClaimsPrincipal claimsPrincipal)
        {
            List<OrgAppUserRoleClaim> orgAppRoles = claimsPrincipal.GetClaims<OrgAppUserRoleClaim>("orgAppRoles");

            foreach (var org  in orgAppRoles)
	        {
                if(!org.IsActive && org.IsDeleted)
                    continue;
                
                var app = org.Apps.FirstOrDefault(i => App.Equals(i.AppName) && i.IsActive && !i.IsDeleted);

                if(app == null)
                    continue;

                var exists = app.Roles.Any(i => i.IsActive && !i.IsDeleted && AppRoles.Any(ar => i.Name.Equals(ar)));

                return exists;
	        }

            return false;
        }

        private bool IsInOrgRole(ClaimsPrincipal claimsPrincipal)
        {
            List<OrgUserRoleClaim> orgRoles = claimsPrincipal.GetClaims<OrgUserRoleClaim>("orgUserRoles");

            var org = orgRoles.FirstOrDefault(i => i.IsActive && !i.IsDeleted && i.OrgName.ToLower().Equals(Organization));

            if(org == null)
                return false;

            if(!org.OrgRoles.Any(i => i.IsActive && !i.IsDeleted && OrgRoles.Any(r => i.Name.ToLower().Equals(r))))
                return false;

            return true;
        }

        private bool IsInGlobalRole(ClaimsPrincipal claimsPrincipal)
        {
            List<OrgGlobalRoleClaim> items = claimsPrincipal.GetClaims<OrgGlobalRoleClaim>("orgGlobalRoles");

            var org = items.FirstOrDefault(i => i.IsActive && !i.IsDeleted && i.OrgName.ToLower().Equals(Organization));

            if (org == null)
                return false;

            if (!org.GlobalRoles.Any(i => i.IsActive && !i.IsDeleted && GlobalRoles.Any(r => i.Name.ToLower().Equals(r))))
                return false;

            return true;
        }
    }

    public class OrgAppAuthIpFilterAttribute : AuthorizationFilterAttribute
    {
        public string Organization { get; set; }
        public string Application { get; set; }
        public List<string> Roles { get; set; }

        public OrgAppAuthIpFilterAttribute()
        {
            Roles = new List<string>();
        }
        public OrgAppAuthIpFilterAttribute(string organization, string application)
            : this()
        {
            Organization = organization;
            Application = application;
        }

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (actionContext.SkipAuthorization())
                return;

            if (!actionContext.RequestContext.Principal.Identity.IsAuthenticated)
            {
                actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                return;
            }
                
            //var name = WindowsIdentity.GetCurrent().Name;
            
            //since windows authenticated users won't have any of the claims we are looking for just authorize the user
            if (typeof(WindowsIdentity) == actionContext.RequestContext.Principal.Identity.GetType())
                return;


            var claimsPrincipal = actionContext.RequestContext.Principal as ClaimsPrincipal;

            if (!claimsPrincipal.HasClaim(x => x.Type == "orgAppAuthorizedIps") ||
                !claimsPrincipal.HasClaim(x => x.Type == "orgUsers"))
            {
                actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                return;
            }

            if (!IsOrgUserActive(claimsPrincipal) || !IsIpAddressValid(claimsPrincipal))
                actionContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
        }

        private bool IsIpAddressValid(ClaimsPrincipal authorizedIpClaim)
        {
            var ipAddress = HttpContext.Current.Request.UserHostAddress;

            List<OrgAppAuthorizedIpClaim> authorizedIps = authorizedIpClaim.GetClaims<OrgAppAuthorizedIpClaim>("orgAppAuthorizedIps");

            if (!authorizedIps.Any())
                return false;

            var thisOrganization = authorizedIps.FirstOrDefault(i => i.IsOrgActive && !i.IsOrgDeleted && i.OrgName.ToLower().Equals(Organization.ToLower()));

            if (thisOrganization == null)
                return false;

            var app = thisOrganization.OrgApps.FirstOrDefault(i => i.IsActive && !i.IsDeleted && i.AppName.ToLower().Equals(Application.ToLower()));

            if (app == null)
                return false;

            var ips = app.Ips.Where(i => i.IsActive && !i.IsDeleted).ToList();

            if (!ips.Any())
                return false;

            //Split the users IP address into it's 4 octets (Assumes IPv4) 
            string[] incomingOctets = ipAddress.Trim().Split(new char[] { '.' });


            //Iterate through each valid IP address 
            foreach (var validIpAddress in ips)
            {
                //Split the valid IP address into it's 4 octets 
                string[] validOctets = validIpAddress.Ip.Trim().Split(new char[] { '.' });

                bool matches = true;

                //Iterate through each octet 
                for (int index = 0; index < validOctets.Length; index++)
                {
                    //Skip if octet is an asterisk indicating an entire 
                    //subnet range is valid 
                    if (validOctets[index] != "*")
                    {
                        if (validOctets[index] != incomingOctets[index])
                        {
                            matches = false;
                            break; //Break out of loop 
                        }
                    }
                }

                if (matches)
                {
                    return true;
                }
            }

            //Found no matches 
            return false;
        }

        private bool IsOrgUserActive(ClaimsPrincipal orgUsersClaim)
        {
            List<OrgUserClaim> orgUsers = orgUsersClaim.GetClaims<OrgUserClaim>("orgUsers");

            if (orgUsers.Any(i => i.OrgnName.Equals(Organization) && i.IsActive))
                return true;

            return false;
        }
    }
}