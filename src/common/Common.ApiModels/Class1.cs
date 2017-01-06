using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.ApiModels
{
    [JsonObject(MemberSerialization.OptIn)]
    public class LogMessageModel
    {
        [JsonProperty("format")]
        public string Format { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("messages")]
        public List<string> Messages { get; set; }
    }

    public class ModelBase
    {
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public ModelBase() { }
    }

    public class SearchTermModel : ModelBase
    {
        public string searchterm { get; set; }
        public int page { get; set; }
        public int limit { get; set; }
    }

    public class SearchResultItemModel : ModelBase
    {
        public int id { get; set; }
        public string text { get; set; }
        public dynamic metadata { get; set; }
    }

    public class SearchResultModel : ModelBase
    {
        public List<SearchResultItemModel> results { get; set; }
        public bool more { get; set; }
    }

    public class OrgUserModel : ModelBase
    {
        public string Organization { get; set; }
        public string UserName { get; set; }
        public DateTime CreateDateUtc { get; set; }
        public List<string> Links { get; set; }
        public string UserType { get; set; }
    }

    public class ApplicationModel : ModelBase
    {
        public string ApplicationName { get; set; }

        public List<string> Links { get; set; }
    }

    public class ApplicationUserModel: ModelBase
    {
        public string AppName { get; set; }
        public string Username { get; set; }
    }

    public class RoleModel : ModelBase
    {
        public string RoleName { get; set; }
    }

    public class AuthorizedIpModel : ModelBase
    {
        public string Ip { get; set; }

        public string IpType { get; set; }

        public string AuthorizedBy { get; set; }

        public DateTime DateAuthorizedUtc { get; set; }
    }

    public class CorsPolicyModel : ModelBase
    {

        public List<string> Headers { get; set; }

        public List<string> Methods { get; set; }

        public List<string> Origins { get; set; }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class LoginModel : ModelBase
    {
        [JsonProperty("grant_type")]
        public string GrantType { get; set; }
        [JsonProperty("username")]
        public string Username { get; set; }
        [JsonProperty("password")]
        public string Password { get; set; }
        [JsonProperty("organization")]
        public string Organization { get; set; }
        [JsonProperty("application")]
        public string Application { get; set; }
        [JsonProperty("Ip")]
        public string Ip { get; set; }
    }

    public class RegisterModel
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }

    public class ConfiguredUser
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }

        public bool IsDomainUser { get; set; }
        public string OrgName { get; set; }
        public List<string> GlobalRoles { get; set; }
        public Dictionary<string, List<string>> AppRoles { get; set; }
        public Dictionary<string, List<string>> AuthorizedIps { get; set; }

        public ConfiguredUser()
        {
            GlobalRoles = new List<string>();
            AppRoles = new Dictionary<string, List<string>>();
            AuthorizedIps = new Dictionary<string, List<string>>();
        }
        
    }

    public class TokenModel : ModelBase
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }
        [JsonProperty("token_type")]
        public string TokenType { get; set; }
        [JsonProperty("expires_in")]
        public long ExpiresIn { get; set; }
        [JsonProperty("userName")]
        public string UserName { get; set; }
        [JsonProperty(".issued")]
        public DateTime Issued { get; set; }
        [JsonProperty(".expires")]
        public DateTime Expires { get; set; }
    }
}
