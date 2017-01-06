using Common.ApiModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EndpointClients.AuthenticationApi
{
    public class AauthClientSettingsConfigFile : IApiEndpointConfig, IDisposable
    {
        public string BaseAddress { get; set; }

        private string _iisAppName;
        public string IisAppName
        {
            get
            {
                return _iisAppName == null ? "" : _iisAppName;
            }
            set
            {
                _iisAppName = value;
            }
        }
        public string TokenEndpoint { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string DefaultOrganization { get; set; }

        public TokenModel BearerToken { get; set; }

        public AauthClientSettingsConfigFile()
        {
            BearerToken = new TokenModel();

            ReadSettingsFromConfigFile();
        }

        private void ReadSettingsFromConfigFile()
        {
            var appSettings = ConfigurationManager.AppSettings;

            BaseAddress = appSettings["securityApiBaseAddress"];
            IisAppName = appSettings["securityApiIisAppName"];
            TokenEndpoint = appSettings["securityApiTokenEndpoint"];
            Username = appSettings["securityApiUserName"];
            Password = appSettings["securityApiPassword"];
            DefaultOrganization = appSettings["securityApiDefaultOrganization"];
        }

        public void Dispose()
        {

        }
    }
}
