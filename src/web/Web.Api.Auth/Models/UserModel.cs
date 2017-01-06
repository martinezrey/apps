using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Web.Api.Auth.Models
{
    public class UserModel
    {
        public string Username { get; set; }
        public string UserDomainName { get; set; }
        public string UserFullDomainName { get; set; }
        public string AvatarUrl { get; set; }
        public string Url { get; set; }
        public string Name { get; set; }
        public string Company { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int TotalApps { get; set; }
        public int TotalRoles { get; set; }
    }
}