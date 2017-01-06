using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.ApiClaims
{
    public class SimpleRoleClaim
    {
        public string Role { get; set; }

        public bool IsActive { get; set; }

		public int UserId { get; set; }

		public string DomainLogin { get; set; }

		public string UserName { get; set; }

		public string EmailAddress { get; set; }

		public string DepartmentDescription { get; set; }

		public int? DepartmentId { get; set; }
	}

    public class OrgUserRoleClaim
    {
        public string OrgName { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public List<RoleClaim> OrgRoles
        {
            get;
            set;
        }
    }

    public class OrgAppUserRoleClaim
    {
        public string OrgName { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public List<AppClaim> Apps { get; set; }
    }

    public class AppClaim
    {
        public string AppName { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public List<RoleClaim> Roles { get; set; }
    }

    public class OrgUserClaim
    {
        public string OrgnName;
        public string UserName;
        public bool IsActive;
        public bool IsDeleted;
    }

    public class OrgAppAuthorizedIpClaim
    {
        public string OrgName { get; set; }
        public bool IsOrgActive { get; set; }
        public bool IsOrgDeleted { get; set; }
        public List<AppAuthorizedUserIpClaim> OrgApps { get; set; }
    }

    public class OrgAppUserCorsPolicyClaim
    {
        public string Organization { get; set; }
        public List<AppCorsPolicyClaim> AppCorsPolicies { get; set; }
    }

    public class AppCorsPolicyClaim
    {
        public string Application { get; set; }

        public List<string> Origins { get; set; }

        public List<string> Headers { get; set; }

        public List<string> Methods { get; set; }
    }

    public class OrgAppGlobalRoleClaim
    {
        public string Organization { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        public List<AppClaim> OrgApps { get; set; }
    }

    public class OrgGlobalRoleClaim
    {
        public string OrgName { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        public List<RoleClaim> GlobalRoles { get; set; }
    }

    public class AppAuthorizedUserIpClaim
    {
        public string AppName { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public List<AuthorizedIpClaim> Ips { get; set; }
    }

    public class AuthorizedIpClaim
    {
        public string Ip { get; set; }
        public string IpType { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class RoleClaim
    {
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class UserClaims
    {
        public List<OrgUserClaim> UserOrgs { get; set; }
        public List<OrgGlobalRoleClaim> GlobalRoles { get; set; }
        public List<OrgAppUserRoleClaim> OrgAppUserRoles { get; set; }
        public List<OrgUserRoleClaim> OrgUserRoleClaims { get; set; }
        public List<OrgAppAuthorizedIpClaim> OrgAppAuthorizedIps { get; set; }

        public UserClaims()
        {
            GlobalRoles = new List<OrgGlobalRoleClaim>();
            OrgAppUserRoles = new List<OrgAppUserRoleClaim>();
            OrgUserRoleClaims = new List<OrgUserRoleClaim>();
            OrgAppAuthorizedIps = new List<OrgAppAuthorizedIpClaim>();
            UserOrgs = new List<OrgUserClaim>();
        }
    }
}
