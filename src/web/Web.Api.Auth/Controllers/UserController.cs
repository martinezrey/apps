using Common.ApiControllers;
using Common.ApiCors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Web.Api.Auth.Models;

namespace Web.Api.Auth.Controllers
{
    [Authorize]
    [CustomCorsPolicy]
    public class UserController : BaseApiController
    {
        public UserController()
        {

        }

        [Route("api/user")]
        public async Task<HttpResponseMessage> Post(UserModel model)
        {
            //create new user
            return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.OK, new
            {
                Username,
                UserDomainName,
                UserFullDomainName,
                AvatarUrl = "",
                Url = String.Format("/users/{0}", UserDomainName == null ? Username : String.Format("{0}.{1}", UserDomainName, Username)),
                Name = "",
                Company = "",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TotalApps = 3,
                TotalRoles = 23,
            }));
        }

        /// <summary>
        /// Get currently authenticated user
        /// </summary>
        /// <returns></returns>
        [Route("api/user")]
        public async Task<HttpResponseMessage> Get()
        {
            var username = ControllerContext.RequestContext.Principal.Identity.Name.Split('\\')[1];

            return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.OK, new
            {
                UserName = username,
                AvatarUrl = "",
                Url = String.Format("/users/{0}", username),
                Name = "",
                Company = "",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TotalApps = 3,
                TotalRoles = 23,
            }));
        }

        //only allow site admin to call this
        [Route("api/user/{username}")]
        public async Task<HttpResponseMessage> Update(UserModel model)
        {
            //update logic
            return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.NoContent));
        }

        [Route("api/user/{username}")]
        public async Task<HttpResponseMessage> Delete()
        {
            //update logic

            return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.NoContent));
        }

        /* ------ user collection ------ */

        [Route("api/users")]
        public async Task<HttpResponseMessage> Post(List<UserModel> models)
        {
            //create new user
            return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.OK, new
            {
                Username,
                UserDomainName,
                UserFullDomainName,
                AvatarUrl = "",
                Url = String.Format("/users/{0}", UserDomainName == null ? Username : String.Format("{0}.{1}", UserDomainName, Username)),
                Name = "",
                Company = "",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TotalApps = 3,
                TotalRoles = 23,
            }));
        }

        [Route("api/users")]
        public async Task<HttpResponseMessage> Get(string[] username)
        {

            //create new user
            return await Task.FromResult<HttpResponseMessage>(Request.CreateResponse(HttpStatusCode.OK, new
            {
                Username,
                UserDomainName,
                UserFullDomainName,
                AvatarUrl = "",
                Url = String.Format("/users/{0}", UserDomainName == null ? Username : String.Format("{0}.{1}", UserDomainName, Username)),
                Name = "",
                Company = "",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TotalApps = 3,
                TotalRoles = 23,
            }));
        }
    }
}