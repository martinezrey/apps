using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Repository.SqlServerProvider
{
    interface ISystemUser : IUser<int>, IEntity<int>, IDisposable
    {

    }
}
