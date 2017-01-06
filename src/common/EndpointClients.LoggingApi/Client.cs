using Common.Logging;
using Common.ApiModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EndpointClients.LoggingApi
{
    public class LoggingApiClient : BaseAuthClient
    {
        public LogMessageEndpointRepo EndpointLogMessage;

        public LoggingApiClient(IApiEndpointConfig settings, ILogServiceAsync<ILogServiceSettings> logService)
            : base(settings, logService)
        {
            EndpointLogMessage = new LogMessageEndpointRepo(Settings, logService);
        }
    }
}