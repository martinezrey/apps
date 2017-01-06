using Newtonsoft.Json;
using Common.ApiModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using EndpointClients;
using Common.Logging;
using Common.ApiClaims;

namespace EndpointClients.AuthenticationApi
{
    public class AppEndpointRepo : EndpointRepo<ApplicationModel>
    {
        public AppEndpointRepo(IApiEndpointConfig settings, ILogServiceAsync<ILogServiceSettings> logService)
            : base(settings, logService)
        {
            _settings.BaseAddress = settings.BaseAddress;
        }

    }

    public class AppUserEndpointRepo : EndpointRepo<ApplicationUserModel>
    {
        public AppUserEndpointRepo(IApiEndpointConfig settings, ILogServiceAsync<ILogServiceSettings> logService)
            : base(settings, logService)
        {
            _settings.BaseAddress = settings.BaseAddress;
        }
    }

    public class AuthorizedIpEndpointRepo : EndpointRepo<AuthorizedIpModel>
    {
        public AuthorizedIpEndpointRepo(IApiEndpointConfig settings, ILogServiceAsync<ILogServiceSettings> logService)
            : base(settings, logService)
        {
            _settings.BaseAddress = settings.BaseAddress;
        }
    }

    public class CorsPolicyEndpointRepo : EndpointRepo<CorsPolicyModel>
    {
        public CorsPolicyEndpointRepo(IApiEndpointConfig settings, ILogServiceAsync<ILogServiceSettings> logService)
            : base(settings, logService)
        {
            _settings.BaseAddress = settings.BaseAddress;
        }
    }

    public class RolesEndpointRepo : EndpointRepo<RoleModel>
    {
        public RolesEndpointRepo(IApiEndpointConfig settings, ILogServiceAsync<ILogServiceSettings> logService)
            : base(settings, logService)
        {
            _settings.BaseAddress = settings.BaseAddress;
        }
    }

    public class OrgAppUserClaimsEndpointRepo : EndpointRepo<UserClaims>
    {
        public OrgAppUserClaimsEndpointRepo(IApiEndpointConfig settings, ILogServiceAsync<ILogServiceSettings> logService)
            : base(settings, logService)
        {
            _settings.BaseAddress = settings.BaseAddress;
        }

        public async Task<UserClaims> CreateAsync(string endpointFormat, TokenModel model,params string[] urlParameters)
        {
            var endpoint = String.Format(endpointFormat, urlParameters).Replace(" ", "-");
            var requestId = Guid.NewGuid().ToString();

            var client = await GetClient();

            var response = await client.PostAsJsonAsync<TokenModel>(String.Format("{0}/{1}", _settings.IisAppName, endpoint), model);

            var userClaims = await response.Content.ReadAsAsync<UserClaims>();

            return userClaims;
        }
    }

    public class UserClaimsEndpointRepo : EndpointRepo<UserClaims>
    {
        public UserClaimsEndpointRepo(IApiEndpointConfig settings, ILogServiceAsync<ILogServiceSettings> logService)
            : base(settings, logService)
        {
            _settings.BaseAddress = settings.BaseAddress;
        }

        public async Task<UserClaims> CreateAsync(string endpointFormat, TokenModel model, string urlParameters)
        {
            var endpoint = String.Format(endpointFormat, urlParameters).Replace(" ", "-");
            var requestId = Guid.NewGuid().ToString();

            var client = await GetClient();

            client.DefaultRequestHeaders.Add("Authorization", model.AccessToken);


            var response = await client.PostAsJsonAsync<TokenModel>(String.Format("{0}/{1}", _settings.IisAppName, endpoint), model);

            var userClaims = await response.Content.ReadAsAsync<UserClaims>();

            return userClaims;
        }
    }
}
