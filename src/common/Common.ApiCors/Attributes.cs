//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net.Http;
//using System.Text;
//using System.Threading.Tasks;
//using System.Web.Cors;
//using System.Web.Http;
//using System.Web.Http.Cors;
//using Extensions.WebApiDependencyResolverExtension;
//using System.Security.Claims;
//using System.Net;
//using Newtonsoft.Json;
//using Common.ApiModels;

//namespace Common.ApiCors
//{
//    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
//    public class AllowCorsAttribute : Attribute, ICorsPolicyProvider
//    {
//        private CorsPolicy _policy;

//        public AllowCorsAttribute()
//        {
//            _policy = new CorsPolicy
//            {
//                AllowAnyMethod = true,
//                AllowAnyHeader = true,
//                AllowAnyOrigin = true,
//                SupportsCredentials = true,
//            };
//        }

//        public Task<CorsPolicy> GetCorsPolicyAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
//        {
//            CorsPolicy corsPolicy = new CorsPolicy
//                {
//                    AllowAnyMethod = true,
//                    AllowAnyHeader = true,
//                    AllowAnyOrigin = true,
//                    SupportsCredentials = true,
//                };


//            return Task.FromResult<CorsPolicy>(corsPolicy);
//        }
//    }
//}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Cors;
using System.Web.Http;
using System.Web.Http.Cors;
using Extensions.WebApiDependencyResolverExtension;
using System.Security.Claims;
using System.Net;
using Newtonsoft.Json;
using Common.ApiModels;
using System.Web;
using System.Threading;

namespace Common.ApiCors
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class CustomCorsPolicyAttribute : Attribute, ICorsPolicyProvider
    {
        private CorsPolicy _policy;

        public CustomCorsPolicyAttribute()
        {
            // Create a CORS policy.
            _policy = new CorsPolicy
            {
                AllowAnyMethod = true,
                AllowAnyHeader = true,
                AllowAnyOrigin = true,
                SupportsCredentials = true,
            };

            // Add allowed origins.
            //_policy.Origins.Add("http://localhost:61078/");
            //_policy.Origins.Add("http://www.contoso.com");

        }

        public Task<CorsPolicy> GetCorsPolicyAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_policy);
        }
    }

    public class CorsPolicyFactory : ICorsPolicyProviderFactory
    {
        ICorsPolicyProvider _provider = new CustomCorsPolicyAttribute();

        public ICorsPolicyProvider GetCorsPolicyProvider(HttpRequestMessage request)
        {
            return _provider;
        }
    } 
}
