using Common.Logging;
using Common.ApiModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace EndpointClients.LoggingApi
{
    public class LogMessageEndpointRepo : EndpointRepo<LogMessageModel>
    {
        public LogMessageEndpointRepo(IApiEndpointConfig settings, ILogServiceAsync<ILogServiceSettings> logService)
            : base(settings, logService)
        {

        }

        public async Task<HttpResponseMessage> Create(LogMessageModel model)
        {
            return await base.CreateAsync("api/logmessages", model);
        }
    }
}
