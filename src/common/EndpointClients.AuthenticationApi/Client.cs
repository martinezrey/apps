using Newtonsoft.Json;
using Common.ApiModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using Common.Logging;
using Common.ApiClaims;

namespace EndpointClients.AuthenticationApi
{
    public class AuthenticationApiClient : BaseAuthClient, IAuthClient, IDisposable
    {
        private AppEndpointRepo _appRepo;
        private AppUserEndpointRepo _appUserRepo;
        private AuthorizedIpEndpointRepo orgAppUserAuthIpRepo;
        private RolesEndpointRepo _orgAppUserRoleRepo;
        private OrgAppUserClaimsEndpointRepo _orgAppUserClaimsRepo;
        private UserClaimsEndpointRepo _userClaimsRepo;

        public AuthenticationApiClient(IApiEndpointConfig settings, ILogServiceAsync<ILogServiceSettings> logService)
            : base(settings, logService)
        {
            _appRepo = new AppEndpointRepo(Settings, logService);
            _appUserRepo = new AppUserEndpointRepo(Settings, logService);
            orgAppUserAuthIpRepo = new AuthorizedIpEndpointRepo(Settings, logService);
            _orgAppUserRoleRepo = new RolesEndpointRepo(Settings, logService);
            _userClaimsRepo = new UserClaimsEndpointRepo(Settings, logService);
            _orgAppUserClaimsRepo = new OrgAppUserClaimsEndpointRepo(Settings, logService);
        }

        /************* Glboal Roles ***********/

        /************* Application ***********/

        public async Task<List<ApplicationModel>> GetAllOrgAppsAsync(string org = null)
        {
            return await _appRepo.GetAllAsync("api/organizations/{0}/applications", org == null ? Settings.DefaultOrganization : org);
        }

        public async Task<ApplicationModel> GetOrgAppAsync(string app, string org = null)
        {
            return await _appRepo.GetAsync("api/organizations/{0}/applications/{1}",
                                        org == null ? Settings.DefaultOrganization : org,
                                        app);
        }

        public async Task<HttpResponseMessage> UpdateOrgAppAsync<T>(T model, string org = null) where T : ApplicationModel
        {
            return await _appRepo.UpdateAsync("api/organizations/{0}/applications",
                                              model,
                                              org == null ? Settings.DefaultOrganization : org);
        }

        public async Task<ApplicationModel> DeleteOrgAppAsync(string app, string org = null)
        {
            return await _appRepo.GetAsync("api/organizations/{0}/applications/{1}",
                                        org == null ? Settings.DefaultOrganization : org,
                                        app);
        }

        public async Task<HttpResponseMessage> CreateOrgAppAsync<T>(T model, string org = null) where T : ApplicationModel
        {
            return await _appRepo.CreateAsync("api/organizations/{0}/applications",
                                        model,
                                        org == null ? Settings.DefaultOrganization : org);
        }

        /************* Application User***********/

        public async Task<List<ApplicationUserModel>> GetAllOrgAppUsersAsync(string app, string org = null)
        {
            return await _appUserRepo.GetAllAsync("api/organizations/{0}/applications/{1}/users", org == null ? Settings.DefaultOrganization : org,
                                                                                    app);
        }

        public async Task<ApplicationUserModel> GetOrgAppUserAsync(string app, string username, string org = null)
        {
            return await _appUserRepo.GetAsync("api/organizations/{0}/applications/{1}/users/{2}",
                                        org == null ? Settings.DefaultOrganization : org,
                                        app,
                                        username);
        }

        public async Task<HttpResponseMessage> UpdateOrgAppUserAsync<T>(string app, T model, string org = null) where T : ApplicationUserModel
        {
            return await _appUserRepo.UpdateAsync("api/organizations/{0}/applications/{1}/users",
                                              model,
                                              org == null ? Settings.DefaultOrganization : org,
                                              app);
        }

        public async Task<ApplicationUserModel> DeleteOrgAppUserAsync(string app, string username, string org = null)
        {
            return await _appUserRepo.GetAsync("api/organizations/{0}/applications/{1}/users/{2}",
                                        org == null ? Settings.DefaultOrganization : org,
                                        app,
                                        username);
        }

        public async Task<HttpResponseMessage> CreateOrgAppUserAsync<T>(string app, T model, string org = null) where T : ApplicationUserModel
        {
            return await _appUserRepo.CreateAsync("api/organizations/{0}/applications/{1}/users",
                                        model,
                                        org == null ? Settings.DefaultOrganization : org,
                                        app);
        }

        /************* Application User Authorized Ip***********/

        public async Task<List<AuthorizedIpModel>> GetOrgAppUserAuthIps(string app, string username, string org = null)
        {
            return await orgAppUserAuthIpRepo.GetAllAsync("api/organizations/{0}/applications/{1}/users/{2}/authorizedips",
                                        org == null ? Settings.DefaultOrganization : org,
                                        app,
                                        username);
        }

        public async Task<AuthorizedIpModel> GetOrgAppUserAuthIp(string app, string username, string ip, string org = null)
        {
            return await orgAppUserAuthIpRepo.GetAsync("api/organizations/{0}/applications/{1}/users/{2}/authorizedips/{3}",
                                        org == null ? Settings.DefaultOrganization : org,
                                        app,
                                        username,
                                        ip);
        }

        public async Task<HttpResponseMessage> UpdateOrgAppAuthIpAsync<T>(string app, string username, string ip, T model, string org = null) where T : AuthorizedIpModel
        {
            return await orgAppUserAuthIpRepo.UpdateAsync("api/organizations/{0}/applications/{1}/users/{2}/authorizedips/{3}",
                                              model,
                                              org == null ? Settings.DefaultOrganization : org,
                                              app,
                                              username,
                                              ip);
        }

        public async Task<HttpResponseMessage> DeleteOrgAppAuthIpAsync(string app, string username, string ip, string org = null)
        {
            return await orgAppUserAuthIpRepo.DeleteAsync("api/organizations/{0}/applications/{1}/users/{2}/authorizedips/{3}", org, app, username, ip);
        }

        public async Task<HttpResponseMessage> CreateOrgAppUserAuthIp<TAuthIpModel>(string app, string username, TAuthIpModel model, string org = null) where TAuthIpModel : AuthorizedIpModel
        {
            return await orgAppUserAuthIpRepo.CreateAsync("api/organizations/{0}/applications/{1}/users/{2}/authorizedips",
                                              model,
                                              org == null ? Settings.DefaultOrganization : org,
                                              app,
                                              username);
        }

        /************* Application User Roles***********/

        public async Task<RoleModel> GetOrgAppUserRoleAsync(string app, string username, string roleName, string org = null)
        {
            return await _orgAppUserRoleRepo.GetAsync("api/organizations/{0}/applications/{1}/users/{2}/roles/{3}",
                                        org == null ? Settings.DefaultOrganization : org,
                                        app,
                                        username,
                                        roleName);
        }

        public async Task<List<RoleModel>> GetOrgAppUserRolesAsync(string app, string username, string org = null)
        {
            return await _orgAppUserRoleRepo.GetAllAsync("api/organizations/{0}/applications/{1}/users/{2}/roles",
                                        org == null ? Settings.DefaultOrganization : org,
                                        app,
                                        username);
        }

        public async Task<HttpResponseMessage> UpdateOrgAppUserRoleAsync<T>(string app, string username, string roleName, T model, string org = null) where T : RoleModel
        {
            return await _orgAppUserRoleRepo.UpdateAsync("api/organizations/{0}/applications/{1}/users/{2}/roles/{3}",
                                              model,
                                              org == null ? Settings.DefaultOrganization : org,
                                              app,
                                              username,
                                              roleName);
        }

        public async Task<HttpResponseMessage> DeleteOrgAppUserRolesAsync(string app, string username, string roleName, string org = null)
        {
            return await _orgAppUserRoleRepo.DeleteAsync("api/organizations/{0}/applications/{1}/users/{2}/roles/{3}",
                                              org == null ? Settings.DefaultOrganization : org,
                                              app,
                                              username,
                                              roleName);
        }

        public async Task<HttpResponseMessage> CreateOrgAppUserRoleAync<T>(string app, string username, T model, string org = null) where T : RoleModel
        {
            return await _orgAppUserRoleRepo.CreateAsync("api/organizations/{0}/applications/{1}/users/{2}/roles",
                                              model,
                                              org == null ? Settings.DefaultOrganization : org,
                                              app,
                                              username);
        }

        /*************User Claims***********/

        public async Task<UserClaims>GetUserClaims<T>(T model)  where T : TokenModel
        {
            var endpoint = "api/claims";

            var client = new HttpClient();

            client.BaseAddress = new Uri(Settings.BaseAddress);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("Authorization", model.AccessToken);
           
            var response = await client.GetAsync(String.Format("{0}/{1}", Settings.IisAppName, endpoint));

            var userClaims = await response.Content.ReadAsAsync<UserClaims>();

            _logService.LogMessage(new
            {
                type = "GetUserClaims",
                clientType = _logService.GetType().FullName,
                msg = "claims retrieved",
                data = userClaims,
            });

            return userClaims;
        }

        public void Dispose()
        {
            
        }
    }
}
