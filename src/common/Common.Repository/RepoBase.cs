using Common.Repository;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Extensions.Objects;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;

namespace Common.Repository
{
    public abstract class RepoBase<TEntity> : IRepository<TEntity>, IDisposable
            where TEntity : class
    {
        private event EventHandler<string> _onDbActivity;
        public event EventHandler<string> OnDbActivity
        {
            add
            {
                _onDbActivity += value;
            }
            remove
            {
                _onDbActivity -= value;
            }
        }
        private event EventHandler<string> _onDbError;
        public event EventHandler<string> OnDbError
        {
            add
            {
                _onDbError += value;
            }
            remove
            {
                _onDbError -= value;
            }
        }

        private DbContext _context;

        private IDbSet<TEntity> _entities
        {
            get
            {
                return _context.Set<TEntity>();
            }
        }

        public IQueryable<TEntity> Entities { get { return _entities; } }

        private string _errorMessage = string.Empty;

        public RepoBase(DbContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("Null DbContext");
            }

            _context = context;
            _context.Database.Log = (i) =>
            {
                if (_onDbActivity != null)
                    _onDbActivity(this, i);
            };
        }

        protected virtual IQueryable<TEntity> GetQuery(Expression<Func<TEntity, bool>> filter = null, int page = 0, int limit = 0, params string[] includeProperties)
        {
            IQueryable<TEntity> query = _entities;

            if (filter != null)
                query = _entities.Where(filter);

            if (includeProperties.Length != 0)
                query = query.Include(string.Join(",", includeProperties));

            if (page != 0)
                query.Skip(limit * page).Take(limit);

            return query;
        }

        public virtual IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> filter = null, 
                       Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, 
                       int page = 0,
                       int limit = 0,
                       params string[] includeProperties)
        {
            IEnumerable<TEntity> results = null;

            try
            {
                IQueryable<TEntity> query = GetQuery(filter, page, limit, includeProperties);

                if (orderBy != null)
                    results = orderBy(query).ToList();

                results = query.ToList();
            }
            catch (DbEntityValidationException dbEx)
            {
                RaiseDbValidationError(dbEx);
            }
            catch (Exception ex)
            {
                RaiseDbException(ex);
            }

            return results;
        }

        public virtual async Task<IEnumerable<TEntity>> GetAsync(Expression<Func<TEntity, bool>> filter = null,
               Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
               int page = 0,
               int limit = 0,
               params string[] includeProperties)
        {
            IEnumerable<TEntity> results = null;

            try
            {
                IQueryable<TEntity> query = GetQuery(filter, page, limit, includeProperties);

                if (orderBy != null)
                    results = await orderBy(query).ToListAsync();

                results = await query.ToListAsync();
            }
            catch (DbEntityValidationException dbEx)
            {
                RaiseDbValidationError(dbEx);
            }
            catch (Exception ex)
            {
                RaiseDbException(ex);
            }

            return results;
        }

        public virtual void Create(TEntity entity)
        {
            try
            {
                if (entity == null)
                {
                    throw new ArgumentNullException("entity");
                }
                _entities.Add(entity);

                _context.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                RaiseDbValidationError(dbEx, entity);
            }
            catch (Exception ex)
            {
                RaiseDbException(ex, entity);
            }
        }

        public virtual void Create(TEntity[] entities)
        {
            try
            {
                if (entities == null)
                    throw new ArgumentNullException("entities");

                ((DbSet<TEntity>)_entities).AddRange(entities);
                _context.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                RaiseDbValidationError(dbEx: dbEx, type: entities.FirstOrDefault(), parameters: entities);
            }
            catch (Exception ex)
            {
                RaiseDbException(ex: ex, type: entities.FirstOrDefault(), parameters: entities);
            }
        }

        public virtual async Task CreateAsync(TEntity entity)
        {
            try
            {
                if (entity == null)
                {
                    throw new ArgumentNullException("entity");
                }

                _entities.Add(entity);

                await _context.SaveChangesAsync();
            }
            catch (DbEntityValidationException dbEx)
            {
                RaiseDbValidationError(dbEx, entity);
            }
            catch (Exception ex)
            {
                RaiseDbException(ex, entity);
            }

            await Task.FromResult<TEntity>(entity);
        }

        public virtual void CreateAsync(TEntity[] entities)
        {
            try
            {
                if (entities == null)
                    throw new ArgumentNullException("entities");

                ((DbSet<TEntity>)_entities).AddRange(entities);
                _context.SaveChangesAsync();
            }
            catch (DbEntityValidationException dbEx)
            {
                RaiseDbValidationError(dbEx: dbEx, type: entities.FirstOrDefault(), parameters: entities);
            }
            catch (Exception ex)
            {
                RaiseDbException(ex: ex, type: entities.FirstOrDefault(), parameters: entities);
            }
        }

        public virtual void Update(TEntity entity)
        {
            try
            {
                _entities.Attach(entity);
                _context.Entry(_entities).State = EntityState.Modified;
                _context.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                RaiseDbValidationError(dbEx, entity);
            }
            catch (Exception ex)
            {
                RaiseDbException(ex);
            }
        }

        public virtual void Update(TEntity[] entities)
        {
            try
            {
                foreach (var item in entities)
                    _entities.Attach(item);
                
                _context.Entry(_entities).State = EntityState.Modified;
                _context.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                RaiseDbValidationError(dbEx: dbEx, type: entities.FirstOrDefault(), parameters: entities);
            }
            catch (Exception ex)
            {
                RaiseDbException(ex);
            }
        }

        public virtual async Task UpdateAsync(TEntity entity)
        {
            try
            {
                _entities.Attach(entity);
                _context.Entry(_entities).State = EntityState.Modified;
                _context.SaveChanges();

                await _context.SaveChangesAsync();
                
            }
            catch (DbEntityValidationException dbEx)
            {
                RaiseDbValidationError(dbEx, entity);
            }
            catch (Exception ex)
            {
                RaiseDbException(ex, entity);
            }

            await Task.FromResult<TEntity>(entity);
        }

        public virtual void UpdateAsync(TEntity[] entities)
        {
            try
            {
                foreach (var item in entities)
                {
                    _entities.Attach(item);
                }

                _context.Entry(_entities).State = EntityState.Modified;
                _context.SaveChangesAsync();
            }
            catch (DbEntityValidationException dbEx)
            {
                RaiseDbValidationError(dbEx: dbEx, type: entities.FirstOrDefault(), parameters: entities);
            }
            catch (Exception ex)
            {
                RaiseDbException(ex);
            }
        }

        public virtual void Delete(TEntity entity)
        {
            try
            {
                if (entity == null)
                {
                    throw new ArgumentNullException("entity");
                }

                this._entities.Remove(entity);
                _context.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                RaiseDbValidationError(dbEx, entity);
            }
            catch (Exception ex)
            {
                RaiseDbException(ex, entity);
            }
        }

        public virtual async Task DeleteAsync(TEntity entity)
        {
            try
            {
                if (entity == null)
                {
                    throw new ArgumentNullException("entity");
                }

                this._entities.Remove(entity);
                await _context.SaveChangesAsync();
            }
            catch (DbEntityValidationException dbEx)
            {
                RaiseDbValidationError(dbEx, entity);
            }
            catch (Exception ex)
            {
                RaiseDbException(ex, entity);
            }

            await Task.FromResult<TEntity>(entity);
        }

        public virtual async Task DeleteAsync(TEntity[] entities)
        {
            try
            {
                foreach (var item in entities)
                    _entities.Remove(item);

                await _context.SaveChangesAsync();
            }
            catch (DbEntityValidationException dbEx)
            {
                RaiseDbValidationError(dbEx: dbEx, type: entities.FirstOrDefault(), parameters: entities);
            }
            catch (Exception ex)
            {
                RaiseDbException(ex);
            }
        }

        public virtual TEntity Find(params object[] parameters)
        {
            TEntity entity = null;

            try
            {
                entity = _entities.Find(parameters);
            }
            catch (DbEntityValidationException dbEx)
            {
                RaiseDbValidationError(dbEx, parameters: parameters);
            }
            catch (Exception ex)
            {
                RaiseDbException(ex, parameters: parameters);
            }

            return entity;
        }

        public virtual async Task<TEntity> FindAsync(params object[] parameters)
        {
            TEntity entity = null;

            try
            {
                entity = this._entities.Find(parameters);
            }
            catch (DbEntityValidationException dbEx)
            {
                RaiseDbValidationError(dbEx, method: "");
                RaiseDbValidationError(dbEx, parameters: parameters);
            }
            catch (Exception ex)
            {
                RaiseDbException(ex, parameters: parameters);
            }

            return await Task.FromResult<TEntity>(entity);
        }

        public virtual ObjectQuery<DbDataRecord> ExecuteQuery(string query, params ObjectParameter[] parameters)
        {
            ObjectQuery<DbDataRecord> objectQuery = new ObjectQuery<DbDataRecord>(query, ((IObjectContextAdapter)_context).ObjectContext);

            try
            {
                foreach (var param in parameters)
                {
                    objectQuery.Parameters.Add(param);
                }
            }
            catch (DbEntityValidationException dbEx)
            {
                RaiseDbValidationError(dbEx, query: query);
            }
            catch (Exception ex)
            {
                RaiseDbException(ex, query: query);
            }

            return objectQuery;
        }

        public virtual ObjectResult<TReturnType> ExecuteScalarFunc<TReturnType>(string funcName, params ObjectParameter[] parameters)
        {
            ObjectResult<TReturnType> result = null;

            try
            {
                result = ((IObjectContextAdapter)_context).ObjectContext.ExecuteFunction<TReturnType>(funcName, parameters);
            }
            catch (DbEntityValidationException dbEx)
            {
                RaiseDbValidationError(dbEx, query: funcName, parameters: parameters);
            }
            catch (Exception ex)
            {
                RaiseDbException(ex, query: funcName, parameters: parameters);
            }

            return result;
        }

        public virtual ObjectResult<TReturnType> ExecuteSproc<TReturnType>(string sprocName, params ObjectParameter[] parameters)
        {
            return ExecuteScalarFunc<TReturnType>(sprocName, parameters);
        }


        private void RaiseDbValidationError(DbEntityValidationException dbEx, 
            TEntity type = null, 
            string query = null, 
            object[] parameters = null, 
            [CallerMemberName] string method = null,
            [CallerFilePath] string sourceFilePath = "", 
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            _errorMessage += String.Format("{0}", dbEx.Message);

            Dictionary<string, Object> entityParamValues = new Dictionary<string, Object>();

            if (type != null)
                entityParamValues = type.ToPropertyNameValues<TEntity>();

            if (query != null)
                _errorMessage += String.Format("{0}{1}", Environment.NewLine, query);

            if(parameters != null)
            {
                for (int i = 0; i < parameters.Length; i++)
                {
                    entityParamValues.Add(String.Format("Parameter  {0}", i), parameters[i]);
                }
            }

            foreach (var validationErrors in dbEx.EntityValidationErrors)
            {
                foreach (var validationError in validationErrors.ValidationErrors)
                {
                    _errorMessage += String.Format("{0}Property: {1} Error: {2}", Environment.NewLine, validationError.PropertyName, validationError.ErrorMessage);
                }
            }

            var serializedMsg = JsonConvert.SerializeObject(new
                {
                    dateThrown = DateTime.Now,
                    type = "dbValidationException",
                    msg = String.Format("{0} Exception", method),
                    data = new
                    {
                        errorMessage = _errorMessage,
                        dbEntityValidationException = dbEx,
                        parameters = entityParamValues,
                    },
                    notification = new
                    {
                        Task = String.Format("Performing {0} in source file {1} code line {2}", method, sourceFilePath, sourceLineNumber),
                        Reason = _errorMessage,
                        Parameters = entityParamValues,
                        Detail = dbEx.StackTrace,
                    }
                }, new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    Formatting = Formatting.None
                });

            if (_onDbError != null)
                _onDbError(this, serializedMsg);

            throw new Exception("DB Validation Exception", new Exception(serializedMsg, dbEx));
        }

        private void RaiseDbException(Exception ex, TEntity type = null, string query = null, object[] parameters = null, [CallerMemberName] string method = null, [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            _errorMessage += String.Format("{0}", ex.Message);

            Dictionary<string, Object> entityParamValues = new Dictionary<string, Object>();

            if (type != null)
                entityParamValues = type.ToPropertyNameValues<TEntity>();

            if (query != null)
                _errorMessage += String.Format("{0} ", query);

            if (parameters != null)
            {
                for (int i = 0; i < parameters.Length; i++)
                {
                    entityParamValues.Add(String.Format("Parameter  {0}", i), parameters[i]);
                }
            }

            string serializedMsg = null;

            try
            {
                serializedMsg = JsonConvert.SerializeObject(new
                {
                    dateThrown = DateTime.Now,
                    type = "dbException",
                    msg = String.Format("{0} Exception", method),
                    data = new
                    {
                        errorMessage = _errorMessage,
                        exception = ex,
                        parameters = entityParamValues,
                    },
                    notification = new
                    {
                        Task = String.Format("Performing {0} in source file {1} code line {2}", method, sourceFilePath, sourceLineNumber),
                        Reason = _errorMessage,
                        Parameters = entityParamValues,
                        Detail = ex.StackTrace,
                    }
                }, new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    Formatting = Formatting.None
                });
            }
            catch(Exception jsonEx)
            {
                //there was an error serializing so ignore this exception
            }

            if (_onDbError != null)
                _onDbError(this, serializedMsg);

            throw new Exception("DB Exception", new Exception(serializedMsg, ex));
        }

        /// <summary>
        /// Disposes the current object
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes all external resources.
        /// </summary>
        /// <param name="disposing">The dispose indicator.</param>
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_context != null)
                {
                    _context.Dispose();
                    _context = null;
                }
            }
        }
    }
}
