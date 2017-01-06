using Common.ApiModels;
using Common.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace EndpointClients
{
    public class EndpointRepo<TApiModel> where TApiModel : class
    {
        protected IApiEndpointConfig _settings;
        protected ILogServiceAsync<ILogServiceSettings> _logService;

        public EndpointRepo(IApiEndpointConfig settings, ILogServiceAsync<ILogServiceSettings> logService)
        {
            _settings = settings;
            _logService = logService;
        }

        public virtual async Task<TApiModel> GetAsync(string endpointFormat, params string[] urlParameters)
        {
            var requestId = Guid.NewGuid().ToString();
            var endpoint = String.Format(endpointFormat, urlParameters).Replace(" ", "-");
            var client = await GetClient();

            var result = await client.GetAsync(String.Format("{0}/{1}", _settings.IisAppName, endpoint)).ContinueWith<Task<TApiModel>>(async response =>
            {
                if (response.Result.IsSuccessStatusCode)
                {
                    var deserializedResult = response.Result.Content.ReadAsAsync<TApiModel>();
                    var serializedResult = await response.Result.Content.ReadAsStringAsync();

                    return await deserializedResult;
                }

                return await Task.FromResult<TApiModel>(default(TApiModel));
            });

            var resposne = await result;

            _logService.LogMessage(new
            {
                type = "endpointClient",
                msg = "GetAllAsync",
                data = new
                {
                    endpoint,
                    settings = _settings,
                    request = new
                    {
                        defaultRequestHeaders = client.DefaultRequestHeaders.ToList(),
                    },
                    resposne,
                },
            });

            return resposne;
        }

        public virtual async Task<List<TApiModel>> GetAllAsync(string endpointFormat, params string[] urlParameters)
        {
            var endpoint = String.Format(endpointFormat, urlParameters).Replace(" ", "-");
            var requestId = Guid.NewGuid().ToString();

            var client = await GetClient();

            var response = await client.GetAsync(String.Format("{0}/{1}", _settings.IisAppName, endpoint)).ContinueWith<Task<List<TApiModel>>>(async r =>
            {
                if (r.Result.IsSuccessStatusCode)
                {
                    var deserializedResult = r.Result.Content.ReadAsAsync<List<TApiModel>>();
                    var serializedResult = await r.Result.Content.ReadAsStringAsync();

                    return await deserializedResult;
                }

                return await Task.FromResult<List<TApiModel>>(new List<TApiModel>());
            });

            var items = await response;

            _logService.LogMessage(new
            {
                type = "endpointClient",
                msg = "GetAllAsync",
                data = new
                {
                    endpoint,
                    settings = _settings,
                    request = new
                    {
                        defaultRequestHeaders = client.DefaultRequestHeaders.ToList(),
                    },
                    response,
                },
            });

            return items;
        }

        public virtual async Task<HttpResponseMessage> CreateAsync(string endpointFormat, TApiModel model, params string[] urlParameters)
        {
            var endpoint = String.Format(endpointFormat, urlParameters).Replace(" ", "-");

            var client = await GetClient();

            var response = await client.PostAsJsonAsync<TApiModel>(String.Format("{0}/{1}", _settings.IisAppName, endpoint), model);

            _logService.LogMessage(new
            {
                type = "endpointClient",
                msg = "CreateAsync",
                data = new
                {
                    endpoint,
                    settings = _settings,
                    request = new
                    {
                        defaultRequestHeaders = client.DefaultRequestHeaders.ToList(),
                    },
                    response = new
                    {
                        headers = response.Headers.ToList(),
                    },
                },
            });

            return response;
        }

        public virtual async Task<HttpResponseMessage> UpdateAsync(string endpointFormat, TApiModel model, params string[] urlParameters)
        {
            var endpoint = String.Format(endpointFormat, urlParameters).Replace(" ", "-");

            var client = await GetClient();

            var response = await client.PutAsJsonAsync<TApiModel>(String.Format("{0}/{1}", _settings.IisAppName, endpoint), model);

            _logService.LogMessage(new
            {
                type = "endpointClient",
                msg = "UpdateAsync",
                data = new
                {
                    endpoint,
                    settings = _settings,
                    request = new
                    {
                        defaultRequestHeaders = client.DefaultRequestHeaders.ToList(),
                    },
                    response = new
                    {
                        headers = response.Headers.ToList(),
                    },
                },
            });

            return response;
        }

        public virtual async Task<HttpResponseMessage> DeleteAsync(string endpointFormat, params string[] urlParameters)
        {
            var endpoint = String.Format(endpointFormat, urlParameters).Replace(" ", "-");

            var client = await GetClient();

            var response = await client.DeleteAsync(String.Format("{0}/{1}", _settings.IisAppName, endpoint));

            _logService.LogMessage(new
            {
                type = "endpointClient",
                msg = "DeleteAsync",
                data = new
                {
                    endpoint,
                    settings = _settings,
                    request = new {
                        defaultRequestHeaders = client.DefaultRequestHeaders.ToList(),
                    },
                    response = new {
                        headers = response.Headers.ToList(),
                    },
                },
            });

            return response;
        }

        public virtual async Task RefreshTokenAsync()
        {
            var client = new HttpClient();
            string endpoint = null;

            if (String.IsNullOrEmpty(_settings.IisAppName))
                endpoint = _settings.TokenEndpoint;
            else
                endpoint = String.Format("{0}/{1}", _settings.IisAppName, _settings.TokenEndpoint);


            client.BaseAddress = new Uri(_settings.BaseAddress);
            client.DefaultRequestHeaders.Accept.Clear();
            //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));


            var response = await client.PostAsync(_settings.TokenEndpoint, new StringContent(String.Format("grant_type=refresh_token&refresh_token={0}", _settings.BearerToken.RefreshToken)));

            _settings.BearerToken = JsonConvert.DeserializeObject<TokenModel>(await response.Content.ReadAsStringAsync());

            _logService.LogMessage(new
                {
                    type = "endpointClient",
                    msg = "RefreshTokenAsync",
                    data = new
                    {
                        endpoint,
                        settings = _settings,
                        request = new
                        {
                            defaultRequestHeaders = client.DefaultRequestHeaders.ToList(),
                        },
                        response = new
                        {
                            headers = response.Headers.ToList(),
                        },
                    },
                });
        }

        protected async Task<HttpClient> GetClient()
        {
            if (_settings.BearerToken.AccessToken != null && DateTime.UtcNow >=_settings.BearerToken.Expires)
                await RefreshTokenAsync();

            var client = new HttpClient();

            client.BaseAddress = new Uri(_settings.BaseAddress);
            client.DefaultRequestHeaders.Accept.Clear();

            if ((_settings.BearerToken.AccessToken != null))
                client.DefaultRequestHeaders.Add("Authorization", String.Format("Bearer {0}", _settings.BearerToken.AccessToken));

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return client;
        }
    }
}
