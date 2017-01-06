using Common.ApiControllers;
using Common.Logging;
using System.Web.Http;

namespace Web.Api
{
    [Authorize]
    public class HealthController : HealthControllerBase
    {
        public HealthController() : base() { }

        public HealthController(ILogServiceAsync<ILogServiceSettings> logService)
            : base(logService)
        {

        }

    }
}