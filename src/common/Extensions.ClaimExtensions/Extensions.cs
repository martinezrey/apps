using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Extensions.ClaimExtensions
{
    public static class ClaimsPrincipalExtension
    {
        public static Claim FirstOrDefault(this ClaimsPrincipal claimsPrincipal, string claimType)
        {
            try
            {
                return claimsPrincipal.FindFirst(claimType);
            }
            catch
            {
                return null;
            }
        }

        public static List<T> GetClaims<T>(this ClaimsPrincipal claim, string claimName)
        {
            List<T> items = new List<T>();

            var claimValue = claim.FirstOrDefault(claimName);

            if (claimValue != null)
                items.AddRange(JsonConvert.DeserializeObject<List<T>>(claimValue.Value));

            return items;
        }

        public static T GetClaim<T>(this ClaimsPrincipal claim, string claimName)
        {
            T t = default(T);

            var claimValue = claim.FirstOrDefault(claimName);

            if (claimValue != null)
                t = JsonConvert.DeserializeObject<T>(claimValue.Value);

            return t;
        }
    }
}
