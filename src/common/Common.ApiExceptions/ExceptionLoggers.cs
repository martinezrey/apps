using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using Extensions.WebApiDependencyResolverExtension;
using Common.Logging;
using System.Threading;
using System.Security.Claims;
using Extensions.ClaimExtensions;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using Common.Notification;
using Common.ApiClaims;


namespace Common.ApiExceptions
{
    //public class UnhandledApiException
    //{
    //    public DateTime DateThrown { get; set; }
    //    public string Type { get; set; }
    //    public string Endpoint { get; set; }
    //    public string Ip { get; set; }
    //    public string Endpoint { get; set; }
    //    public string Username { get; set; }
    //    public string IsAuthenticated { get; set; }
    //    public string ControllerFullName { get; set; }
    //    public Dictionary<string, object> RouteData { get; set; }
    //    public Exception UnhandledException { get; set; }
    //    public HttpRequestHeaders Headers { get; set; }
    //}

    public class ClaimsExceptionLogger : ExceptionLogger
    {
        public override Task LogAsync(ExceptionLoggerContext context, CancellationToken cancellationToken)
        {
            var logService = GlobalConfiguration.Configuration.DependencyResolver.GetService<ILogServiceAsync<ILogServiceSettings>>();

            return Task.Factory.StartNew(async () =>
                {
                    try
                    {
                        var content = await context.Request.Content.ReadAsStringAsync();

                        var claimsPrincipal = context.RequestContext.Principal as ClaimsPrincipal;

                        logService.LogMessage(new
                        {
                            type = "exception",
                            endpoint = context.Request.RequestUri,
                            userName = context.RequestContext.Principal.Identity.Name,
                            authorizeClaims = new
                            {
                                orgAppAuthorizedIps = claimsPrincipal.GetClaims<OrgAppAuthorizedIpClaim>("orgAppAuthorizedIps"),
                                orgAppRoles = claimsPrincipal.GetClaims<OrgAppUserRoleClaim>("orgAppRoles"),
                                orgGlobalRoles = claimsPrincipal.GetClaims<OrgGlobalRoleClaim>("orgGlobalRoles"),
                                orgUserRoles = claimsPrincipal.GetClaims<OrgUserRoleClaim>("orgUserRoles"),
                            },
                            data = new
                            {
                                message = String.Format("Exception thrown"),
                                exception = context.ExceptionContext.Exception,
                            }
                        });
                    }
                    catch
                    {

                    }
                });
        }
    }

    public class DefaultExceptionLogger : ExceptionLogger
    {
        private event EventHandler<string> _onLogException;
        public event EventHandler<string> OnLogException
        {
            add
            {
                _onLogException += value;
            }
            remove
            {
                _onLogException -= value;
            }
        }

        public DefaultExceptionLogger(EventHandler<string> onLogException) :base()
        {
            _onLogException += onLogException;
        }

        public override Task LogAsync(ExceptionLoggerContext context, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(async () =>
            {
                try
                {
                    if (_onLogException == null)
                        return;

                    string ip = null;

                    if (context.Request.Properties.ContainsKey("MS_HttpContext"))
                        ip = ((dynamic)context.Request.Properties["MS_HttpContext"]).Request.UserHostAddress;

                    var actionContext = ((ApiController)context.ExceptionContext.ControllerContext.Controller).ActionContext;

                    var serializedItem = JsonConvert.SerializeObject(new
                    {
                        dateThrown = DateTime.Now,
                        type = "exception",
                        request = new 
                        {
                            endpoint = context.Request.RequestUri,
                            ip,
                            httpMethod = context.Request.Method.Method,
                            user = new 
                            {
                                name = context.RequestContext.Principal.Identity.Name,
                                isAuthenticated = context.RequestContext.Principal.Identity.IsAuthenticated,
                            },
                            controller = new
                            {
                                type = context.ExceptionContext.ControllerContext.ControllerDescriptor.ControllerType.FullName,
                                routeTemplate = context.ExceptionContext.ControllerContext.RouteData.Route.RouteTemplate,
                                actionArguments = actionContext.ActionArguments,
                                //controller = actionContext.ControllerContext.Controller;
                                controllerDescription = actionContext.ControllerContext.ControllerDescriptor,
                            },
                            headers = context.Request.Headers,
                        },
                        exception = context.Exception,
                        notification = new
                        {
                            Username = context.RequestContext.Principal.Identity.Name,
                            Task = String.Format("Performing {0} in controller {1}", actionContext.ActionDescriptor.ActionName, actionContext.ControllerContext.Controller.GetType().Name),
                            Reason = context.Exception.Message,
                            Parameters = actionContext.ActionArguments,
                            Detail = context.Exception.StackTrace,
                        }
                    }, new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                        Formatting = Formatting.None
                    });

                    _onLogException(this, serializedItem);
                }
                catch (Exception ex)
                {
                    //don't stop execution since we already handling all exceptions here if it failes to log we don't care
                }
            });
        }

        public static void HandleOnExceptionLoggged(object source, string serializedArgs)
        {
            var logService = GlobalConfiguration.Configuration.DependencyResolver.GetService<ILogServiceAsync<ILogServiceSettings>>();

            var notificationClient = GlobalConfiguration.Configuration.DependencyResolver.GetService<INotificationClient>();

            logService.LogMessage(serializedArgs);


            if (notificationClient == null)
                return;

            dynamic unhandledException = JsonConvert.DeserializeObject(serializedArgs);

            NotificationException notification = unhandledException.notification.ToObject<NotificationException>();

            notification.RawMessage = unhandledException;
        }
    }
}
