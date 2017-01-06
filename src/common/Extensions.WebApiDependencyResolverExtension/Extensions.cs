using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extensions.WebApiDependencyResolverExtension
{
    public static class DependencyResolverExtension
    {
        public static T GetService<T>(this System.Web.Http.Dependencies.IDependencyScope scope)
        {
            return (T)scope.GetService(typeof(T));
        }
    }
}