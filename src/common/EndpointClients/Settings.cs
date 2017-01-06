using Common.ApiModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EndpointClients
{
    public class EndpointConfig : IApiEndpointConfig
    {
        public string IisAppName { get; set; }
        public string BaseAddress { get; set; }
        public string TokenEndpoint { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string DefaultOrganization { get; set; }

        public TokenModel BearerToken { get; set; }

        public EndpointConfig()
        {
            
        }
    }
}
