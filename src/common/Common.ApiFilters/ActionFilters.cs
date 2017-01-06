using Common.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Extensions.WebApiDependencyResolverExtension;
using System.Security.Claims;
using Newtonsoft.Json;

using Extensions.ClaimExtensions;
using Common.ApiClaims;

namespace Common.ApiFilters
{
    public class LogActionFilter : ActionFilterAttribute
    {
        private ILogServiceAsync<ILogServiceSettings> _logService;
        public ILogServiceAsync<ILogServiceSettings> LogService
        {
            get
            {
                if (_logService == null)
                    _logService = GlobalConfiguration.Configuration.DependencyResolver.GetService<ILogServiceAsync<ILogServiceSettings>>();

                return _logService;
            }
            set
            {
                _logService = value;
            }
        }

        public override Task OnActionExecutingAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    var claimsPrincipal = actionContext.RequestContext.Principal as ClaimsPrincipal;

                    string ip = null;

                    if (actionContext.Request.Properties.ContainsKey("MS_HttpContext"))
                        ip = ((dynamic)actionContext.Request.Properties["MS_HttpContext"]).Request.UserHostAddress;

                    LogService.LogMessage(new
                    {
                        type = "actionExecutingLogEntry",
                        ip,
                        endPoint = actionContext.Request.RequestUri,
                        userName = actionContext.RequestContext.Principal.Identity.Name,
                        authorizeClaims = new
                        {
                            orgAppAuthorizedIps = claimsPrincipal.GetClaims<OrgAppAuthorizedIpClaim>("orgAppAuthorizedIps"),
                            orgAppRoles = claimsPrincipal.GetClaims<OrgAppUserRoleClaim>("orgAppRoles"),
                            orgGlobalRoles = claimsPrincipal.GetClaims<OrgGlobalRoleClaim>("orgGlobalRoles"),
                            orgUserRoles = claimsPrincipal.GetClaims<OrgUserRoleClaim>("orgUserRoles"),
                        },
                        data = new
                        {
                            actionContext.ActionDescriptor.ActionName,
                            actionContext.ActionArguments,
                            actionContext.ActionDescriptor.Properties,
                            actionContext.ActionDescriptor.ReturnType,
                            actionContext.ActionDescriptor.SupportedHttpMethods,
                            request = new
                            {
                                actionContext.Request.RequestUri,
                                actionContext.Request.Headers,
                                actionContext.Request.Method,
                            },
                        }
                    });
                }
                catch(Exception ex)
                {
                    LogService.LogMessage(new
                    {
                        type = "exception",
                        ex = ex,
                    });
                }
            });
        }

        public override Task OnActionExecutedAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(async ()=>
            {
                try
                {
                    var claimsPrincipal = actionExecutedContext.ActionContext.RequestContext.Principal as ClaimsPrincipal;

                    var responseData = actionExecutedContext.Response == null || actionExecutedContext.Response.Content == null ? null : await actionExecutedContext.Response.Content.ReadAsStringAsync();
                    string ip = null;

                    if(actionExecutedContext.Request.Properties.ContainsKey("MS_HttpContext"))
                        ip = ((dynamic)actionExecutedContext.Request.Properties["MS_HttpContext"]).Request.UserHostAddress;

                    LogService.LogMessage(new
                    {
                        type = "actionExecutedLogEntry",
                        ip,
                        endPoint = actionExecutedContext.Request.RequestUri,
                        userName = actionExecutedContext.ActionContext.RequestContext.Principal.Identity.Name,
                        authorizeClaims = new
                        {
                            orgAppAuthorizedIps = claimsPrincipal.GetClaims<OrgAppAuthorizedIpClaim>("orgAppAuthorizedIps"),
                            orgAppRoles = claimsPrincipal.GetClaims<OrgAppUserRoleClaim>("orgAppRoles"),
                            orgGlobalRoles = claimsPrincipal.GetClaims<OrgGlobalRoleClaim>("orgGlobalRoles"),
                            orgUserRoles = claimsPrincipal.GetClaims<OrgUserRoleClaim>("orgUserRoles"),
                        },
                        responseData,
                    });
                }
                catch (Exception ex)
                {
                    LogService.LogMessage(new 
                    {
                        type = "exception",
                        ex = ex,
                    });
                }
            });
        }
    }

    public class InstrumentationActionFilter : ActionFilterAttribute
    {
        private ILogServiceAsync<ILogServiceSettings> _logService;
        public ILogServiceAsync<ILogServiceSettings> LogService
        {
            get
            {
                if (_logService == null)
                    _logService = GlobalConfiguration.Configuration.DependencyResolver.GetService<ILogServiceAsync<ILogServiceSettings>>();

                return _logService;
            }
            set
            {
                _logService = value;
            }
        }

        private Stopwatch stopwatch = new Stopwatch();

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            stopwatch.Start();
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            stopwatch.Stop();

            LogService.LogMessage(new
            {
                type = "Instrumentation",
                endpoint = actionExecutedContext.Request.RequestUri,
                userName = actionExecutedContext.ActionContext.RequestContext.Principal.Identity.Name,
                data = new
                {
                    timeElapsedMilliseconds = stopwatch.ElapsedMilliseconds,
                    timeElapsedInSeconds = stopwatch.ElapsedMilliseconds * 0.001,
                    timeElapsedInMinutes = (stopwatch.ElapsedMilliseconds * 0.001) / 60,
                },
            });

            stopwatch.Reset();
        }
    }
}
