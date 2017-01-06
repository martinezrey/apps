using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mfcu.Repositories.ITGov
{
    public partial class ITGovEntities : DbContext
    {
        public ITGovEntities(DbConnection existingConnection, bool contextOwnsConnection)
            : base(existingConnection, contextOwnsConnection)  
        {  
  
        }
    }
}
