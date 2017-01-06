using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extensions.Owin
{
    public static class OwinExtensions
    {
        public static T ToLifetimeScope<T>(this IOwinContext owinContext, string environmentVariable) where T : class
        {
            if (!owinContext.Environment.Any(a => a.Key.Contains(environmentVariable) && a.Value != null))
                throw new Exception("RequestScope cannot be null...");

            return owinContext.Environment.FirstOrDefault(f => f.Key.Contains(environmentVariable)).Value as T;
        }
    }
}
