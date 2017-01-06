using Common.ApiClaims;
using Common.ApiModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace EndpointClients.AuthenticationApi
{
    public interface IAuthClient//<out TApiModel> where TApiModel : ModelBase
    {
        IApiEndpointConfig Settings { get; }

        Task<bool> LoginAsync();
        Task<bool> RegisterAsync(RegisterModel model);

        /************* Application ***********/

        Task<List<ApplicationModel>> GetAllOrgAppsAsync(string organization = null);

        Task<ApplicationModel> GetOrgAppAsync(string application, string organization = null);

        Task<HttpResponseMessage> UpdateOrgAppAsync<T>(T model, string organization = null) where T : ApplicationModel;

        Task<ApplicationModel> DeleteOrgAppAsync(string application, string organization = null);

        Task<HttpResponseMessage> CreateOrgAppAsync<T>(T model, string organization = null) where T : ApplicationModel;

        /************* Application User***********/

        Task<List<ApplicationUserModel>> GetAllOrgAppUsersAsync(string application, string organization = null);

        Task<ApplicationUserModel> GetOrgAppUserAsync(string application, string username, string organization = null);

        Task<HttpResponseMessage> UpdateOrgAppUserAsync<T>(string app, T model, string organization = null) where T : ApplicationUserModel;

        Task<ApplicationUserModel> DeleteOrgAppUserAsync(string application, string username, string organization = null);

        Task<HttpResponseMessage> CreateOrgAppUserAsync<T>(string app, T model, string organization = null) where T : ApplicationUserModel;

        /************* Application User Authorized Ip***********/

        Task<List<AuthorizedIpModel>> GetOrgAppUserAuthIps(string application, string username, string organization = null);

        Task<AuthorizedIpModel> GetOrgAppUserAuthIp(string application, string username, string ip, string organization = null);

        Task<HttpResponseMessage> UpdateOrgAppAuthIpAsync<T>(string application, string username, string ip, T model, string organization = null) where T : AuthorizedIpModel;

        Task<HttpResponseMessage> DeleteOrgAppAuthIpAsync(string application, string username, string ip, string organization = null);

        Task<HttpResponseMessage> CreateOrgAppUserAuthIp<TAuthIpModel>(string application, string username, TAuthIpModel model, string organization = null) where TAuthIpModel : AuthorizedIpModel;

        /************* Application User Roles***********/

        Task<RoleModel> GetOrgAppUserRoleAsync(string application, string username, string roleName, string organization = null);

        Task<List<RoleModel>> GetOrgAppUserRolesAsync(string application, string username, string organization = null);

        Task<HttpResponseMessage> UpdateOrgAppUserRoleAsync<T>(string application, string username, string roleName, T model, string organization = null) where T : RoleModel;

        Task<HttpResponseMessage> DeleteOrgAppUserRolesAsync(string application, string username, string roleName, string organization = null);

        Task<HttpResponseMessage> CreateOrgAppUserRoleAync<T>(string application, string username, T model, string organization = null) where T : RoleModel;

        /************* User Claims***********/

        Task<UserClaims> GetUserClaims<T>(T model) where T : TokenModel;
    }
}
