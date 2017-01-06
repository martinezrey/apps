using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Common.Repository
{
    public interface IRepository<TEntity> where TEntity : class
    {
        event EventHandler<string> OnDbActivity;
        event EventHandler<string> OnDbError;
    } 
}
