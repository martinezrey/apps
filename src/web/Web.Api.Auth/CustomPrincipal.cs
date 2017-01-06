using Common.ApiClaims;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace Web.Api.Auth
{
    public class CustomPrincipal : GenericPrincipal
    {
        public SimpleRoleClaim Role { get; set; }

        public CustomPrincipal(IIdentity identity,
            SimpleRoleClaim roleClaim)

            : base(identity, new List<string>().ToArray())
        {
            Role = roleClaim;
        }
    }
}