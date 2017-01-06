using Common.ApiFilters;
using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Filters;

namespace Common.ApiControllers
{
    public class HealthControllerBase : BaseApiController
    {
        public HealthControllerBase() { }

        public HealthControllerBase(ILogServiceAsync<ILogServiceSettings> logService)
        {
            _logService = logService;
        }

        [Route("api/health")]
        public async Task<HttpResponseMessage> Get()
        {
            var uitems = Configuration.Services.GetApiExplorer().ApiDescriptions.ToList();

            var health = new
            {
                isAlive = true,
                totalUniqueEndpoints = Configuration.Services.GetApiExplorer().ApiDescriptions.Select(i => i.RelativePath).Distinct().Count(),
                uniqueEndpoints = Configuration.Services.GetApiExplorer().ApiDescriptions.Select(i => i.RelativePath).Distinct(),
                totalEndpoints = Configuration.Services.GetApiExplorer().ApiDescriptions.Count,
                endpoints = Configuration.Services.GetApiExplorer().ApiDescriptions.OrderBy(i => i.RelativePath).Select(i => new
                {
                    url = i.RelativePath,
                    controllerName = i.ActionDescriptor.ControllerDescriptor.ControllerName,
                    requiresAuthentication = i.ActionDescriptor.ControllerDescriptor.GetCustomAttributes<AuthorizeAttribute>().Any() ||
                                             i.ActionDescriptor.ControllerDescriptor.GetCustomAttributes<AuthorizationFilterAttribute>().Any(),
                    validRoles = i.ActionDescriptor.ControllerDescriptor.GetCustomAttributes<RoleAuthorizeFilter>(false).Select(x => new
                    {
                        organzation = x.Organization,
                        globalRoles = x.GlobalRoles,
                        orgRoles = x.OrgRoles,
                        app = x.App,
                        appRoles = x.AppRoles
                    }).ToList(),
                    supportedHttpMethods = i.ActionDescriptor.SupportedHttpMethods,
                    parameterDescriptions = i.ParameterDescriptions.Select(pd => new
                    {
                        name = pd.Name,
                        source = pd.Source.ToString(),
                        //parameterDescriptor = pd.ParameterDescriptor.Properties,
                    }),
                }).ToList(),
            };

            return Request.CreateResponse(await Task.FromResult<HttpStatusCode>(HttpStatusCode.OK), health);
        }
    }
}
