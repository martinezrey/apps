using Autofac;
using Autofac.Integration.WebApi;
using Microsoft.Owin;
using Microsoft.Owin.Security.Infrastructure;
using Microsoft.Owin.Security.OAuth;
using Owin;
using System;
using System.Configuration;
using System.Reflection;
using System.Web.Http;

[assembly: OwinStartup(typeof(Web.Api.Startup))]

namespace Web.Api
{
    public class Startup
    {
        public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }

        public static string PublicClientId { get; private set; }

        public void Configuration(IAppBuilder app)
        {
            var builder = new ContainerBuilder();

            // get HttpConfiguration
            var config = GlobalConfiguration.Configuration;

            // register Web API controllers
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            // OPTIONAL: Register the Autofac filter provider.
            builder.RegisterWebApiFilterProvider(config);

            Bootstrapper.RegisterAutofac(builder);

            // set the dependency resolver to be Autofac
            var container = builder.Build();
            app.UseAutofacMiddleware(container);
            app.UseAutofacWebApi(config);
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);

            ConfigureOAuth(app);

            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            app.UseWebApi(config);
        }

        private void ConfigureOAuth(IAppBuilder app)
        {
            Bootstrapper.RegisterOwin(app);

            // Configure the application for OAuth based flow
            PublicClientId = "self";
            OAuthOptions = new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new PathString("/Token"),
                Provider = new ApplicationOAuthProvider(PublicClientId),
                AuthorizeEndpointPath = new PathString("/api/Account/ExternalLogin"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(Convert.ToInt32(ConfigurationManager.AppSettings["security.accesstokenexpirationindays"])), //TimeSpan.FromDays(14),//TODO: configure token expiration time in web config
                //RefreshTokenProvider = new RefreshTokenProvider(),
                //RefreshTokenProvider = new AuthenticationTokenProvider()
                //{
                //    OnCreate = CreateRefreshToken,
                //    OnReceive = RecieveRefreshToken  
                //},
                AllowInsecureHttp = true
            };

            // Enable the application to use bearer tokens to authenticate users
            app.UseOAuthBearerTokens(OAuthOptions);
        }

        private static void RecieveRefreshToken(AuthenticationTokenReceiveContext obj)
        {
            //throw new NotImplementedException();  
            obj.DeserializeTicket(obj.Token);
        }

        private static void CreateRefreshToken(AuthenticationTokenCreateContext obj)
        {
            // throw new NotImplementedException();  
            obj.SetToken(obj.SerializeTicket());
        }
    }
}
