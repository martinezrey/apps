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
using Extensions.StringExtensions;

namespace EndpointClients
{
    public abstract class BaseAuthClient
    {
        protected ILogServiceAsync<ILogServiceSettings> _logService;
        public IApiEndpointConfig Settings { get; private set; }

        public BaseAuthClient(IApiEndpointConfig settings)
        {
            Settings = settings;
            _logService = LogServiceAsync<LogServiceOptions>.Instance;
        }

        public BaseAuthClient(IApiEndpointConfig settings, ILogServiceAsync<ILogServiceSettings> logService)
        {        
            _logService = logService;
            Settings = settings;
        }

        public async Task<bool> LoginAsync()
        {
            await RefreshTokenAsync();

            var client = await GetClient();
            var endpoint = String.Format("{0}/{1}", Settings.IisAppName, Settings.TokenEndpoint);
            var postData = new StringContent(String.Format("grant_type=password&username={0}&password={1}", Settings.Username, Settings.Password));

            client.BaseAddress = new Uri(Settings.BaseAddress);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            _logService.LogMessage(new
                                  {
                                      msg = "wtf",
                                      type = "authenticationclient",
                                      data = new
                                      {
                                          endpoint,
                                          postData = String.Format("grant_type=password&username={0}&password={1}", Settings.Username, Settings.Password),
                                          client.BaseAddress,
                                          client.DefaultRequestHeaders,
                                      }
                                  });

            var response = await client.PostAsync(endpoint, postData);

            _logService.LogMessage(new
            {
                msg = "response retrieved",
                type = "authenticationclient",
                data = new
                {
                    endpoint,
                }
            });

            if (!response.IsSuccessStatusCode)
            {
                _logService.LogMessage(new
                {
                    type = "authenticationclient",
                    clientType = _logService.GetType().FullName,
                    msg = "LoginAsync failed",
                    data = new
                    {
                        request = new
                        {
                            response.RequestMessage.RequestUri,
                            response.RequestMessage.Headers,
                            response.RequestMessage.Method,
                            response.RequestMessage.Properties,
                        },
                        response = new
                        {
                            response.ReasonPhrase,
                            response.Headers,
                            response.IsSuccessStatusCode,
                            response.StatusCode,
                            response.Version,
                        },
                        settings = Settings,
                    }
                });
                return false;
            }

            var resultContent = await response.Content.ReadAsStringAsync();

            Settings.BearerToken = JsonConvert.DeserializeObject<TokenModel>(resultContent);

            _logService.LogMessage(new
            {
                type = "authenticationclient",
                clientType = _logService.GetType().FullName,
                msg = "token endpoint called",
                data = new
                {
                    request = new
                    {
                        response.RequestMessage.RequestUri,
                        response.RequestMessage.Headers,
                        response.RequestMessage.Method,
                        response.RequestMessage.Properties,
                    },
                    response = new
                    {
                        body = resultContent,
                        response.ReasonPhrase,
                        response.Headers,
                        response.IsSuccessStatusCode,
                        response.StatusCode,
                        response.Version,
                    },
                    settings = Settings,
                }
            });

            if (Settings.BearerToken == null)
                return false;

            _logService.LogMessage(new
            {
                type = "authenticationclient",
                clientType = _logService.GetType().FullName,
                msg = "token retrieved",
                data = Settings,
            });

            return true;
        }

        public async Task<Boolean> RegisterAsync(RegisterModel model)
        {
            model.Password += "a!";
            model.ConfirmPassword += "a!";

            var requestId = Guid.NewGuid().ToString();

            var client = new HttpClient();

            client.BaseAddress = new Uri(Settings.BaseAddress);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await client.PostAsJsonAsync<RegisterModel>(Settings.IisAppName.IsNullOrEmptyOrWhiteSpace() ? 
                "api/account/register" :
                String.Format("{0}/api/account/register", Settings.IisAppName), model);

            //if (!response.IsSuccessStatusCode)
            //LogServiceAsync<LogServiceOptions>.Instance.Write("{0} POST: failed {1}", requestId, response.StatusCode);
            //else
            //LogServiceAsync<LogServiceOptions>.Instance.Write("{0} POST: succeeded {1}", requestId, response.StatusCode);

            _logService.LogMessage(new
            {
                type = "authenticationclient",
                clientType = _logService.GetType().FullName,
                msg = "register called",
                data = new 
                {
                    registerModel = model,
                    succeeded = response.IsSuccessStatusCode
                },
            });

            return response.IsSuccessStatusCode;
        }

        public virtual async Task RefreshTokenAsync()
        {
            if (Settings.BearerToken.Expires.Year == 1 || Settings.BearerToken.Expires > DateTime.UtcNow)
                return;
            
            var client = new HttpClient();
            var endpoint = String.Format("{0}{1}", Settings.IisAppName == null ? String.Empty : Settings.IisAppName, Settings.TokenEndpoint);

            client.BaseAddress = new Uri(Settings.BaseAddress);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));

            var postTask = await client.PostAsync(Settings.TokenEndpoint, new StringContent(String.Format("grant_type=refresh_token&refresh_token={0}", Settings.BearerToken.RefreshToken)));

            Settings.BearerToken = JsonConvert.DeserializeObject<TokenModel>(await postTask.Content.ReadAsStringAsync());

            _logService.LogMessage(new
            {
                type = "authenticationclient",
                clientType = _logService.GetType().FullName,
                msg = "token refreshed",
                data = Settings
            });
        }

        protected async Task<HttpClient> GetClient()
        {
            await RefreshTokenAsync();

            var client = new HttpClient();

            client.BaseAddress = new Uri(Settings.BaseAddress);
            client.DefaultRequestHeaders.Accept.Clear();

            if (Settings.BearerToken != null && Settings.BearerToken.AccessToken != null)
                client.DefaultRequestHeaders.Add("Authorization", String.Format("Bearer {0}", Settings.BearerToken.AccessToken));

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return client;
        }
    }
}
