using Common.ApiModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EndpointClients
{
    public interface IApiEndpointConfig
    {
        string BaseAddress { get; set; }
        string IisAppName { get; set; }
        string TokenEndpoint { get; set; }
        string Username { get; set; }
        string Password { get; set; }
        string DefaultOrganization { get; set; }

        TokenModel BearerToken { get; set; }
    }
}
