using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Common.ApiCors;

namespace Common.ApiControllers
{
    //[EnableCors(origins: "mfcu.com", headers: "*", methods: "*")]

    //[CustomCorsPolicyAttribute(organization:"mfcu", app: "authentication.api")]

    [CustomCorsPolicy]
    public class BaseApiController : ApiController
    {
        protected virtual string UserFullDomainName
        {
            get
            {
                if (!ControllerContext.RequestContext.Principal.Identity.IsAuthenticated)
                    return null;

                return ControllerContext.RequestContext.Principal.Identity.Name;
            }
        }

        protected virtual string Username
        {
            get
            {
                if (!ControllerContext.RequestContext.Principal.Identity.IsAuthenticated)
                    return null;

                string username = ControllerContext.RequestContext.Principal.Identity.Name;

                if (username.Contains("//"))
                    return username.Split('\\')[1];

                return username;
            }
        }

        protected virtual string UserDomainName
        {
            get
            {
                if (!ControllerContext.RequestContext.Principal.Identity.IsAuthenticated)
                    return null;

                string username = ControllerContext.RequestContext.Principal.Identity.Name;

                if (username.Contains("//"))
                    return username.Split('\\')[0];

                return username;
            }
        }

        protected ILogServiceAsync<ILogServiceSettings> _logService;

        public BaseApiController() { }

        public BaseApiController(ILogServiceAsync<ILogServiceSettings> logService)
        {
            _logService = logService;
        }
    }
}
