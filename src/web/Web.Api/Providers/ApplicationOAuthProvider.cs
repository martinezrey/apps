using Common.ApiClaims;
using Common.Logging;
using Common.Repository;
using Extensions.ClaimExtensions;
using Extensions.WebApiDependencyResolverExtension;
using IdentityProviders.SqlServerProvider;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;

namespace Web.Api
{
    public class ApplicationOAuthProvider : OAuthAuthorizationServerProvider
    {
        private readonly string _publicClientId;

        public ApplicationOAuthProvider(string publicClientId)
        {
            if (publicClientId == null)
            {
                throw new ArgumentNullException("publicClientId");
            }

            _publicClientId = publicClientId;
        }

        public static AuthenticationProperties CreateProperties(User user)
        {
            IDictionary<string, string> data = new Dictionary<string, string>
            {
                { "domainLogin", user.UserName },
                { "userName", user.UserName },
                { "fullName", user.Email },
                //{ "emailAddress", user.EmailAddress },
                //{ "departmentDescr", user.DepartmentDescr },
                //{ "isActive", user.boolIsActive.ToString() },
                //{ "systemUserId", user.SystemUserID.ToString() },
                //{ "departmentID", user.DepartmentID.ToString() },
                //{ "role", user.AppRole }
            };

            return new AuthenticationProperties(data);
        }

        //http://discoveringdotnet.alexeyev.org/2014/08/simple-explanation-of-bearer-authentication-for-web-api-2.html
        public override async Task GrantRefreshToken(OAuthGrantRefreshTokenContext context)
        {
            //enforce client binding of refresh token
            if (context.Ticket == null || context.Ticket.Identity == null || !context.Ticket.Identity.IsAuthenticated)
            {
                context.SetError("invalid_grant", "Refresh token is not valid");
            }
            else
            {
                var userIdentity = context.Ticket.Identity;
                var authenticationTicket = await CreateAuthenticationTicket(context.OwinContext, userIdentity);

                //Additional claim is needed to separate access token updating from authentication
                //requests in RefreshTokenProvider.CreateAsync() method
                authenticationTicket.Identity.AddClaim(new Claim("refreshToken", "refreshToken"));

                context.Validated(authenticationTicket);
            }
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            ILogServiceAsync<ILogServiceSettings> logService = GlobalConfiguration.Configuration.DependencyResolver.GetService<ILogServiceAsync<ILogServiceSettings>>();

            var sqlUserManager = context.OwinContext.GetUserManager<ApplicationUserManager>();
            var sqlUser = await sqlUserManager.FindAsync(context.UserName, context.Password);

            if (sqlUser == null || !sqlUser.IsActive)
            {
                context.SetError("invalid_grant", "The user name or password is incorrect.");

                logService.LogMessage(new
                {
                    type = "claimsGenerated",
                    endpoint = context.Request.Uri,
                    userName = context.UserName,
                    data = new
                    {
                        message = "invalid_grant The user name or password is incorrect",
                    }
                });

                return;
            }

            ClaimsIdentity oAuthIdentity = await sqlUser.GenerateUserIdentityAsync(sqlUserManager, OAuthDefaults.AuthenticationType);

            //ClaimsGenerator claimsGenerator = new ClaimsGenerator(
            //    sqlUser,
            //    oAuthIdentity,
            //    context.OwinContext.Get<RepoBase<SystemUser>>());

            //claimsGenerator.GenerateClaims();
            
            //claims generator is the same as this:

            oAuthIdentity.AddClaim(new Claim("projectRequestRole", JsonConvert.SerializeObject(new SimpleRoleClaim
            {
                UserId = 2154,//_user.SystemUserID,
                DomainLogin = "domain loginname",//_user.DomainLogin.Trim(),
                UserName = "someusername",//_user.DomainLogin.Trim(),
                EmailAddress = "yourmom@gmail.com",//_user.EmailAddress.Trim(),
                DepartmentDescription = "some dp des",//_user.DepartmentDescr,
                DepartmentId = 33,//_user.DepartmentID,
                IsActive = true,//_user.boolIsActive,
                Role = "user",//_user.AppRole
            })));

            ClaimsIdentity cookiesIdentity = await sqlUser.GenerateUserIdentityAsync(sqlUserManager, CookieAuthenticationDefaults.AuthenticationType);

            AuthenticationProperties properties = CreateProperties(sqlUser);
            AuthenticationTicket ticket = new AuthenticationTicket(oAuthIdentity, properties);
            ticket.Properties.AllowRefresh = true;
            context.Validated(ticket);
            context.Request.Context.Authentication.SignIn(cookiesIdentity);

            //TODO: Document this
            /*http://stackoverflow.com/questions/21971190/asp-net-web-api-2-owin-authentication-unsuported-grant-type/21979279#21979279
             *********************************
             * it wasn't enough adding config.EnableCors(new EnableCorsAttribute("*", "*", "*")); to WebApiConfig.cs
             * or the controllers. example:
             *     [EnableCors(origins: "*", headers: "*", methods: "*")]
             *     public class ValuesController : ApiController
             */

            var principal = new ClaimsPrincipal(new[] { oAuthIdentity });

            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });

            logService.LogMessage(new
            {
                type = "claimsGenerated",
                endpoint = context.Request.Uri,
                userName = context.UserName,
                data = new
                {
                    orgUsers = principal.GetClaim<SimpleRoleClaim>("projectRequestRole"),
                }
            });
        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            // Resource owner password credentials does not provide a client ID.
            if (context.ClientId == null)
            {
                context.Validated();
            }

            return Task.FromResult<object>(null);

        }

        public override Task ValidateClientRedirectUri(OAuthValidateClientRedirectUriContext context)
        {
            if (context.ClientId == _publicClientId)
            {
                Uri expectedRootUri = new Uri(context.Request.Uri, "/");

                if (expectedRootUri.AbsoluteUri == context.RedirectUri)
                {
                    context.Validated();
                }
            }

            return Task.FromResult<object>(null);
        }

        private async Task<AuthenticationTicket> CreateAuthenticationTicket(IOwinContext owinContext, ClaimsIdentity oAuthIdentity)
        {
            ILogServiceAsync<ILogServiceSettings> logService = GlobalConfiguration.Configuration.DependencyResolver.GetService<ILogServiceAsync<ILogServiceSettings>>();

            var sqlUserManager = owinContext.GetUserManager<ApplicationUserManager>();
            var sqlUser = await sqlUserManager.FindByNameAsync(oAuthIdentity.Name);

            //http://www.c-sharpcorner.com/UploadFile/ff2f08/angularjs-enable-owin-refresh-tokens-using-asp-net-web-api/
            var newIdentity = new ClaimsIdentity(oAuthIdentity);

            //SqlServerClaimsGenerator claimsGenerator = new SqlServerClaimsGenerator(
            //    sqlUser,
            //    oAuthIdentity,
            //    owinContext.Get<RepoBase<SystemUser>>());

            //claimsGenerator.GenerateClaims();

            oAuthIdentity.AddClaim(new Claim("projectRequestRole", JsonConvert.SerializeObject(new SimpleRoleClaim
            {
                UserId = 2154,//_user.SystemUserID,
                DomainLogin = "domain loginname",//_user.DomainLogin.Trim(),
                UserName = "someusername",//_user.DomainLogin.Trim(),
                EmailAddress = "yourmom@gmail.com",//_user.EmailAddress.Trim(),
                DepartmentDescription = "some dp des",//_user.DepartmentDescr,
                DepartmentId = 33,//_user.DepartmentID,
                IsActive = true,//_user.boolIsActive,
                Role = "user",//_user.AppRole
            })));

            var currentUtc = new Microsoft.Owin.Infrastructure.SystemClock().UtcNow;

            AuthenticationProperties properties = CreateProperties(sqlUser);
            AuthenticationTicket ticket = new AuthenticationTicket(newIdentity, properties);
            ticket.Properties.IssuedUtc = DateTime.UtcNow;
            ticket.Properties.ExpiresUtc = currentUtc.Add(TimeSpan.FromDays(365));//TODO: configure token expiration time in web config
            ticket.Properties.AllowRefresh = true;

            var principal = new ClaimsPrincipal(new[] { oAuthIdentity });

            owinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });

            logService.LogMessage(new
            {
                type = "claimsRefreshed",
                endpoint = owinContext.Request.Uri,
                userName = oAuthIdentity.Name,
                data = new
                {
                    orgUsers = principal.GetClaim<SimpleRoleClaim>("projectRequestRole"),
                }
            });

            return ticket;
        }

    }
}