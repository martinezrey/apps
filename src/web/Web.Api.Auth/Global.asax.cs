using Autofac;
using Autofac.Integration.Mvc;
using Common.ApiClaims;
using Common.Logging;
using Extensions.ClaimExtensions;
using Extensions.WebApiDependencyResolverExtension;
using Newtonsoft.Json;
using System;
using System.Net.Http.Formatting;
using System.Reflection;
using System.Security.Claims;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Web.Api.Auth
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configuration.Formatters.Clear();
            GlobalConfiguration.Configuration.Formatters.Add(new JsonMediaTypeFormatter());
            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Serialize;

            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            var builder = new ContainerBuilder();
            // Register your MVC controllers.
            builder.RegisterControllers(typeof(WebApiApplication).Assembly);
            // OPTIONAL: Register model binders that require DI.
            builder.RegisterModelBinders(Assembly.GetExecutingAssembly());
            builder.RegisterModelBinderProvider();
            // OPTIONAL: Register web abstractions like HttpContextBase.
            builder.RegisterModule<AutofacWebTypesModule>();
            // OPTIONAL: Enable property injection in view pages.
            builder.RegisterSource(new ViewRegistrationSource());
            // OPTIONAL: Enable property injection into action filters.
            builder.RegisterFilterProvider();
            Bootstrapper.RegisterAutofac(builder);
            // Set the dependency resolver to be Autofac.
            var container = builder.Build();

            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }

        //http://stackoverflow.com/questions/22724798/mvc-role-authorization
        protected virtual void Application_AuthenticateRequest(object sender, EventArgs args)
        {
            var logService = GlobalConfiguration.Configuration.DependencyResolver.GetService<ILogServiceAsync<ILogServiceSettings>>();

            var usr = HttpContext.Current.User;

            if (Context.User != null)
            {
                var claimsPrincipal = HttpContext.Current.User as ClaimsPrincipal;

                var orgAppRoles = claimsPrincipal.GetClaim<SimpleRoleClaim>("projectRequestRole");

                //we need to set the principal in order to use if(User.IsInRole("system_administrator")) in MVC views. Example : views/manage/Index.cshtml
                CustomPrincipal cp = new CustomPrincipal(Context.User.Identity, orgAppRoles);

                Context.User = cp;
            }
        }
    }
}