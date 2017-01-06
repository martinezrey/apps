using Common.Repository;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.Validation;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Common.Repository.SqlServerProvider
{
    public abstract class SystemUserRepo : RepoBase<SystemUser>
    {
        public SystemUserRepo(DbContext context) : base(context) { }
    }
}