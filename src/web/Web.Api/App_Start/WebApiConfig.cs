using Common.ApiExceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.ExceptionHandling;

namespace Web.Api
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            // Configure Web API to use only bearer token authentication.
            //config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));

            config.Services.Add(typeof(IExceptionLogger), new DefaultExceptionLogger(DefaultExceptionLogger.HandleOnExceptionLoggged));

            config.EnableCors((new EnableCorsAttribute("*", "*", "*")));

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
