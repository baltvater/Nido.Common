using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Data.Entity;
using System.Data;
using Nido.Common.Exceptions;
using System.Data.SqlClient;
using System.Linq.Expressions;
using System.Data.Entity.Infrastructure;
using EntityFramework.Extensions;
using EntityFramework.Caching;
using System.Data.Entity.Validation;
using System.Data.Entity.Core;
using Nido.Common.Extensions;
using EntityFramework.BulkInsert.Extensions;

namespace Nido.Common.BackEnd
{
    /// <summary>
    /// Generic repositor class. 
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    /// <typeparam name="Entity"></typeparam>
    public class DataRepository<TContext, Entity> : IDisposable
        where TContext : BaseObjectConext, IBaseObjectConext
        where Entity : class, IBaseObject, new()
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public DataRepository()
        { }

        /// <summary>
        ///  Cached ObjectSets so changes persist
        /// </summary>
        protected Dictionary<string, object> CachedObjects = new Dictionary<string, object>();

        /// <summary>
        /// Generic Get object set method
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        [Obsolete("GetObjectSet<TEntity>() is deprecated, please use GetObjectSet() instead.")]
        protected DbSet<TEntity> GetObjectSet<TEntity>() where TEntity : class
        {
            var fulltypename = typeof(TEntity).AssemblyQualifiedName;
            if (fulltypename == null)
                throw new ArgumentException("Invalid Type passed to GetObjectSet!");
            if (!CachedObjects.ContainsKey(fulltypename))
            {
                var objectset = _context.Set<TEntity>();
                CachedObjects.Add(fulltypename, objectset);
            }
            return CachedObjects[fulltypename] as DbSet<TEntity>;
        }

        /// <summary>
        /// Targetted Get object set method. 
        /// By default this will get the object set of the corresponding entity object.
        /// </summary>
        /// <returns>DbSet of the entity</returns>
        protected DbSet<Entity> GetObjectSet()
        {
            _entitySet = new ContextAgentFactory<TContext, Entity>().GetContext(context).GetObjectSet();
            return _entitySet;
        }

        private TContext context;

        /// <summary>
        /// The generic DbContext
        /// </summary>
        protected TContext _context
        {
            get { return new ContextAgentFactory<TContext, Entity>().GetContext(context).GetContext(); }
            set { context = value; }
        }

        /// <summary>
        /// Generic DbSet object
        /// </summary>
        protected DbSet<Entity> _entitySet;

        /// <summary>
        /// Include table name queue
        /// </summary>
        protected Queue<string> _includes = new Queue<string>();

        /// <summary>     
        /// Constructor that takes a context     
        /// </summary>     
        /// <param name="context">An established data context</param>     
        public DataRepository(TContext context)
        {
            _context = context;
        }

        /// <summary>     
        /// Constructor that takes a connection string and an EDMX name     
        /// </summary>     
        /// <param name="connectionString">The connection string</param>     
        /// <param name="edmxName">The name of the EDMX so we can build an Entity Connection string</param>     
        public DataRepository(string connectionString, string edmxName)
        {
            var entityConnection = String.Format("metadata=res://*/{0}.csdl|res://*/{0}.ssdl|res://*/{0}.msl;provider=System.Data.SqlClient;provider connection string=", edmxName);
            // append the database connection string and save           
            entityConnection = entityConnection + "\"" + connectionString + "\"";
            var targetType = typeof(TContext); var ctx = Activator.CreateInstance(targetType, entityConnection);
            _context = (TContext)ctx;
        }

        /// <summary>
        /// Generic fetch method
        /// </summary>
        /// <typeparam name="TEntity">Type of the entity to be fetch</typeparam>
        /// <returns>DbSet value of the fetched entity</returns>
        [Obsolete("Fetch<TEntity>() is deprecated, please use Fetch() instead.")]
        protected DbSet<TEntity> Fetch<TEntity>() where TEntity : class
        {
            return GetObjectSet<TEntity>();
        }

        /// <summary>
        /// Generic get all method
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <returns>Enumeralbe entity set</returns>
        [Obsolete("GetAll<TEntity>() is deprecated, please use GetAll() instead.")]
        protected IEnumerable<TEntity> GetAll<TEntity>() where TEntity : class
        {
            return GetObjectSet<TEntity>().ToList().AsEnumerable();
        }

        /// <summary>
        /// Get all the record from the database. Inlcude Supported 
        /// </summary>
        /// <returns></returns>
        protected IQueryable<Entity> GetAllNew()
        {
            try
            {
                if (_includes.Count == 0)
                    return GetObjectSet().AsQueryable().ForEach(item => item.DecryptRecords<Entity>());
                else
                {
                    DbQuery<Entity> entityQuery = GetObjectSet().Include(_includes.Dequeue());
                    while (_includes.Count > 0)
                        entityQuery = entityQuery.Include(_includes.Dequeue());
                    return entityQuery.AsQueryable().ForEach(item => item.DecryptRecords<Entity>());
                }
            }
            catch (ArgumentException e)
            {
                throw new ExceptionDataError(e, ExceptionBase.GetRefId());
            }
            catch (Exception e)
            {
                throw new ExceptionCritical(e, ExceptionBase.GetRefId());
            }
        }

        protected IEnumerable<Entity> GetAllFromCache(CachePolicy cachePolicy = null
            , IEnumerable<string> tags = null)
        {
            try
            {
                if (_includes.Count == 0)
                    if (cachePolicy != null)
                        return GetObjectSet().FromCache(cachePolicy, tags).ForEach(item => item.DecryptRecords<Entity>());
                    else
                        return GetObjectSet().FromCache().ForEach(item => item.DecryptRecords<Entity>());
                else
                {
                    DbQuery<Entity> entityQuery = GetObjectSet().Include(_includes.Dequeue());
                    while (_includes.Count > 0)
                        entityQuery = entityQuery.Include(_includes.Dequeue());
                    if (cachePolicy != null)
                        return entityQuery.FromCache(cachePolicy, tags).ForEach(item => item.DecryptRecords<Entity>());
                    else
                        return entityQuery.FromCache().ForEach(item => item.DecryptRecords<Entity>());
                }
            }
            catch (ArgumentException e)
            {
                throw new ExceptionDataError(e, ExceptionBase.GetRefId());
            }
            catch (Exception e)
            {
                throw new ExceptionCritical(e, ExceptionBase.GetRefId());
            }
        }

        /// <summary>
        /// Get all the matching record from the database. Inlcude Supported 
        /// </summary>
        /// <returns></returns>
        protected IQueryable<Entity> GetAllNew(Expression<Func<Entity, Boolean>> predicate)
        {
            try
            {
                if (_includes.Count == 0)
                    return GetObjectSet().Where(predicate).ForEach(item => item.DecryptRecords<Entity>());
                else
                {
                    DbQuery<Entity> entityQuery = GetObjectSet().Include(_includes.Dequeue());
                    while (_includes.Count > 0)
                        entityQuery = entityQuery.Include(_includes.Dequeue());
                    return entityQuery.Where(predicate).ForEach(item => item.DecryptRecords<Entity>()); 
                }
            }
            catch (ArgumentException e)
            {
                throw new ExceptionDataError(e, ExceptionBase.GetRefId());
            }
            catch (Exception e)
            {
                throw new ExceptionCritical(e, ExceptionBase.GetRefId());
            }
        }

        protected IEnumerable<Entity> GetAllFromCache(Expression<Func<Entity, Boolean>> predicate
            , CachePolicy cachePolicy = null
            , IEnumerable<string> tags = null)
        {
            try
            {
                if (_includes.Count == 0)
                    if (cachePolicy != null)
                        return GetObjectSet().Where(predicate).FromCache(cachePolicy, tags).ForEach(item => item.DecryptRecords<Entity>());
                    else
                        return GetObjectSet().Where(predicate).FromCache().ForEach(item => item.DecryptRecords<Entity>());
                else
                {
                    DbQuery<Entity> entityQuery = GetObjectSet().Include(_includes.Dequeue());
                    while (_includes.Count > 0)
                        entityQuery = entityQuery.Include(_includes.Dequeue());
                    if (cachePolicy != null)
                        return entityQuery.Where(predicate).FromCache(cachePolicy, tags).ForEach(item => item.DecryptRecords<Entity>());
                    else
                        return entityQuery.Where(predicate).FromCache().ForEach(item => item.DecryptRecords<Entity>());
                }
            }
            catch (ArgumentException e)
            {
                throw new ExceptionDataError(e, ExceptionBase.GetRefId());
            }
            catch (Exception e)
            {
                throw new ExceptionCritical(e, ExceptionBase.GetRefId());
            }
        }

        /// <summary>
        /// Find the matching element from the list of database records
        /// </summary>
        /// <typeparam name="TEntity">Type of the entity to be retreived</typeparam>
        /// <param name="predicate">search query to be matched when finding</param>
        /// <returns></returns>
        [Obsolete("Find<TEntity>(Func<TEntity, bool> predicate) is deprecated, please use just Find(Func<Entity, bool> predicate) instead.")]
        protected IEnumerable<TEntity> Find<TEntity>(Expression<Func<TEntity, Boolean>> predicate) where TEntity : class
        {
            return GetObjectSet<TEntity>().Where(predicate);
        }

        //public IEnumerable<Entity> Find(Func<Entity, bool> predicate) 
        //{
        //    return GetObjectSet().Where(predicate);
        //}

        /// <summary>
        /// Retrieve a single object. This method is optimized to use with the primary key.
        /// </summary>
        /// <typeparam name="TEntity">Entity type to be retrieved</typeparam>
        /// <param name="predicate">search query</param>
        /// <returns></returns>
        [Obsolete("GetSingle<TEntity>(Func<TEntity, bool> predicate) is deprecated, please use just GetSingle(Func<Entity, bool> predicate) instead.")]
        protected TEntity GetSingle<TEntity>(Expression<Func<TEntity, Boolean>> predicate) where TEntity : class
        {
            return GetObjectSet<TEntity>().Single(predicate);
        }

        /// <summary>
        /// Select a single matching record. Include Supported. 
        /// If you are selecting by primary key, GetSingleNew is semantically the correct methods to call 
        /// and are usually quite fast.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        protected Entity GetSingleNew(Expression<Func<Entity, Boolean>> predicate)
        {
            if (_includes.Count == 0)
                return GetObjectSet().SingleOrDefault(predicate);
            else
            {
                DbQuery<Entity> entityQuery = GetObjectSet().Include(_includes.Dequeue());
                while (_includes.Count > 0)
                    entityQuery = entityQuery.Include(_includes.Dequeue());
                Entity e = entityQuery.SingleOrDefault(predicate);
                e.DecryptRecords<Entity>();
                return e;
            }
        }

        /// <summary>
        /// Get the first element from the matching query
        /// </summary>
        /// <typeparam name="TEntity">type of the entity to be retrieved</typeparam>
        /// <param name="predicate">matching query</param>
        /// <returns></returns>
        [Obsolete("GetFirst<TEntity>(Func<TEntity, bool> predicate) is deprecated, please use just GetFirst(Func<Entity, bool> predicate) instead.")]
        protected TEntity GetFirst<TEntity>(Expression<Func<TEntity, Boolean>> predicate) where TEntity : class
        {
            return GetObjectSet<TEntity>().First(predicate);
        }

        /// <summary>
        /// Get the first matching record. Include Supported.
        /// Where returns an IQueryable of the same source, equivalent to the corresponding SQL where clause.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        protected Entity GetFirstNew(Expression<Func<Entity, Boolean>> predicate)
        {
            if (_includes.Count == 0)
            {
                Entity e = GetObjectSet().Where(predicate).FirstOrDefault();
                e.DecryptRecords<Entity>();
                return e;
            }
            else
            {
                DbQuery<Entity> entityQuery = GetObjectSet().Include(_includes.Dequeue());
                while (_includes.Count > 0)
                    entityQuery = entityQuery.Include(_includes.Dequeue());

                IEnumerable<Entity> list = entityQuery.Where(predicate);

                Entity e = list.FirstOrDefault();
                e.DecryptRecords<Entity>();

                return e;
            }
        }

        /// <summary>
        /// Add a element to the database
        /// </summary>
        /// <typeparam name="TEntity">entity type to be retrieved from the database</typeparam>
        /// <param name="entity">Enetiy object to be add to the database</param>
        [Obsolete("Add<TEntity>(TEntity entity) is deprecated, please use just Add(Entity entity) instead.")]
        protected void Add<TEntity>(TEntity entity) where TEntity : class
        {
            if (entity == null)
                throw new ArgumentException("Cannot add a null entity"); 
            GetObjectSet<TEntity>().Add(entity);
        }

        /// <summary>
        /// Add object to the database.
        /// This is the recomended method.
        /// </summary>
        /// <param name="entity">The entity object to be added to the database</param>
        /// <remarks>Save method is not called so you have to call it manually. 
        /// If you want to update multiple records and do the save at the end 
        /// then it is recomeded to use methods with 'New' Suffix 
        /// and call the Save method at the end.</remarks>
        protected void AddNew(Entity entity)
        {
            if (entity == null)
                throw new ArgumentException("Cannot add a null entity");

            entity.EncryptRecords<Entity>();           
            GetObjectSet().Add(entity);
        }

        /// <summary>
        /// Bulk insert extension for EntityFramework 4.1.10311+.
        /// Insert large amount of data over 20 times faster than regular insert.
        /// Supports DB first and code first.
        /// </summary>
        /// <param name="entities"></param>
        protected void AddBulkNew(IEnumerable<Entity> entities)
        {
            if ((entities == null) && (entities.Count() > 0))
                throw new ArgumentException("Cannot add a null entity");
            entities.ForEach(item => item.EncryptRecords<Entity>());
            _context.BulkInsert(entities);
        }

        /// <summary>
        /// Delete a record from the database. This will do a cascade 
        /// delete unless you have implement the CanDelete property of 
        /// the respective enetity object.
        /// </summary>
        /// <typeparam name="TEntity">type of the entity to be deleted</typeparam>
        /// <param name="entity">entity object to be used when finding the record to be deleted</param>
        [Obsolete("Delete<TEntity>(TEntity entity) is deprecated, please use just Delete(Entity entity) instead.")]
        protected void Delete<TEntity>(TEntity entity) where TEntity : class
        {
            if (entity == null) throw new ArgumentException("Cannot delete a null entity");
            this.Attach(entity);
            GetObjectSet<TEntity>().Remove(entity);
        }

        /// <summary>
        /// Delete a record from the database. This will do a cascade 
        /// delete unless you have implement the CanDelete property of 
        /// the respective enetity object.
        /// This is the recomended method.
        /// </summary>
        /// <param name="entity">Entity object to be deleted</param>
        /// <remarks>Save method is not called so you have to call it manually. 
        /// If you want to update multiple records and do the save at the end 
        /// then it is recomeded to use methods with 'New' Suffix 
        /// and call the Save method at the end.</remarks> 
        protected void DeleteNew(Entity entity)
        {
            if (entity == null) throw new ArgumentException("Cannot delete a null entity");
            this.AttachNew(entity);
            GetObjectSet().Remove(entity);
        }

        /// <summary>
        /// Delete a record from the database. This will NOT do a cascade 
        /// delete. This will create a SQL out of your command and execute it on the DB.
        /// This method proved to be relatively faster.
        /// This is the recomended method.
        /// </summary>
        /// <param name="entity">Entity object to be deleted</param>
        /// <remarks>Save method is not called so you have to call it manually. 
        /// If you want to update multiple records and do the save at the end 
        /// then it is recomeded to use methods with 'New' Suffix 
        /// and call the Save method at the end.</remarks> 
        protected int DeleteNew(Expression<Func<Entity, bool>> filterExpression)
        {
            if (filterExpression == null) throw new ArgumentException("Cannot delete a null entity");
            return GetObjectSet().Where(filterExpression).Delete();
        }

        /// <summary>
        /// Generic attach method
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        [Obsolete("Attach<TEntity>(TEntity entity) is deprecated, please use just Attach(Entity entity) instead.")]
        protected void Attach<TEntity>(TEntity entity) where TEntity : class
        {
            if (entity == null) throw new ArgumentException("Cannot attach a null entity");
            GetObjectSet<TEntity>().Attach(entity);
        }

        /// <summary>
        /// Attach a entity to the database.
        /// Attaching is required if you do custom update of a object. The generic update method does this internally.
        /// This is the recomended method.
        /// </summary>
        /// <param name="entity">entity to be attached</param>
        protected void AttachNew(Entity entity)
        {
            if (entity == null) throw new ArgumentException("Cannot attach a null entity");
            GetObjectSet().Attach(entity);
        }

        /// <summary>
        /// Generic update method. This will fully update the values of the passing object to the database. No partial update is supported.
        /// </summary>
        /// <typeparam name="TEntity">The type of the generic object to be updated</typeparam>
        /// <param name="entity">Entity object to be update to the database.</param>
        /// <returns></returns>
        [Obsolete("Update<TEntity>(TEntity entity) is deprecated, please use just Update(Entity entity) instead.")]
        protected bool Update<TEntity>(TEntity entity) where TEntity : class
        {
            this.Attach<TEntity>(entity);
            // Copy over new values
            _context.Entry(entity).CurrentValues.SetValues(entity);
            // Set the state to modified.
            _context.Entry(entity).State = System.Data.Entity.EntityState.Modified;
            return true;
        }

        /// <summary>
        /// Update method. 
        /// This will fully update the values of the passing object to the database. 
        /// No partial update is supported.
        /// Save method is not called.
        /// </summary>
        /// <param name="entity">The entity object to be updated</param>
        /// <returns></returns>
        /// <remarks>Save method is not called so you have to call it manually. 
        /// If you want to update multiple records and do the save at the end 
        /// then it is recomeded to use methods with 'New' Suffix 
        /// and call the Save method at the end.</remarks>
        protected bool UpdateNew(Entity entity)
        {
            // Call encryption method to encrypt all records 
            // with the encryption flag
            entity.EncryptRecords<Entity>();
            // Attach the record to the database context object
            this.AttachNew(entity);
            // Copy new values over old values
            _context.Entry(entity).CurrentValues.SetValues(entity);
            // Set the state to modified.
            _context.Entry(entity).State = System.Data.Entity.EntityState.Modified;
            return true;
        }

        /// <summary>
        /// A bulk Update method. Encryption needed to be handled manually.
        /// This will update the values directly executing the query on the DB. 
        /// This support partial updates.
        /// Save method is not called.
        /// </summary>
        /// <param name="entity">The entity object to be updated</param>
        /// <returns></returns>
        /// <remarks>Save method is not called so you have to call it manually. 
        /// If you want to update multiple records and do the save at the end 
        /// then it is recomeded to use methods with 'New' Suffix 
        /// and call the Save method at the end.</remarks>
        /// <example>
        /// <code lang="c#">
        /// // update all tasks with status of 1 to status of 2
        /// context.Tasks.Update(
        ///     t => t.StatusId == 1, 
        ///     t2 => new Task {StatusId = 2});
        /// 
        /// </code>
        /// </example>
        protected bool UpdateNew(Expression<Func<Entity, Entity>> updateExpression)
        {
            GetObjectSet().Update(updateExpression);
            return true;
        }

        /// <summary>
        /// A bulk Update method. 
        /// This will update the values directly executing the query on the DB. 
        /// This support partial updates.
        /// Save method is not called.
        /// </summary>
        /// <param name="filterExpression">Select a set of records from the DB for a bulk update</param>
        /// <param name="updateExpression">Express how the bulk update should happen. 
        /// You may point the fields that needed to be updated in here</param>
        /// <returns></returns>
        /// <example>
        /// <code lang="c#">
        /// // example of using an IQueryable as the filter for the update
        /// var users = context.Users.Where(u => u.FirstName == "firstname");
        /// context.Users.Update(users, u => new User {FirstName = "newfirstname"});
        /// </code>
        /// </example>
        protected bool UpdateNew(Expression<Func<Entity, bool>> filterExpression, Expression<Func<Entity, Entity>> updateExpression)
        {
            GetObjectSet().Update(filterExpression, updateExpression);
            return true;
        }


        /// <summary>
        /// Get the count of records that matches the predicate
        /// </summary>
        /// <param name="predicate">matching predicate</param>
        /// <returns></returns>
        protected int CountNew(Expression<Func<Entity, Boolean>> predicate)
        {
            return GetObjectSet().Count(predicate);
        }

        /// <summary>
        /// Get the count of all the records 
        /// </summary>
        /// <returns></returns>
        protected int CountNew()
        {
            return GetObjectSet().Count();
        }

        /// <summary>
        /// DB save method. This method is called internally. 
        /// But if you want to update multiple tables in a same call then you suppose to handle it manually.
        /// </summary>
        /// <returns>Save status. The number of records updated.</returns>
        /// <exception cref="ExceptionDataError"></exception>
        /// <exception cref="ExceptionRepeatable"></exception>
        /// <exception cref="ExceptionNoneRepeatable"></exception>        
        /// <exception cref="ExceptionCritical"></exception>
        public int SaveChanges()
        {
            try
            {
                int i = _context.SaveChanges();
                return i;
            }
            catch (DbEntityValidationException dbEx)
            {
                StringBuilder s = new StringBuilder("Validation Error: ");
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        s.AppendFormat("\r\nClass: {0}, Property: {1}, Error: {2} ",
                            validationErrors.Entry.Entity.GetType().FullName,
                            validationError.PropertyName,
                            validationError.ErrorMessage);
                    }
                }

                throw new ExceptionDataError(new Exception(s.ToString()), ExceptionBase.GetRefId());
            }
            catch (ArgumentException e)
            {
                throw new ExceptionDataError(e, ExceptionBase.GetRefId());
            }
            catch (System.Data.Entity.Core.OptimisticConcurrencyException e)
            {
                throw new ExceptionRepeatable(e);
            }
            catch (System.Data.Entity.Core.UpdateException e)
            {
                throw new ExceptionDataError(e, ExceptionBase.GetRefId());
            }
            catch (SqlException e)
            {
                throw new ExceptionNoneRepeatable(e, ExceptionBase.GetRefId());
            }
            catch (Exception e)
            {
                throw new ExceptionCritical(e, ExceptionCritical.GetRefId());
            }
        }

        /// <summary>
        /// Get Validation errors
        /// </summary>
        /// <returns>list of validation errors</returns>
        protected IEnumerable<string> getValidationErrors()
        {
            try
            {
                List<string> errList = new List<string>();
                IEnumerable<DbEntityValidationResult> validationResults = _context.GetValidationErrors();
                foreach (DbEntityValidationResult item in validationResults)
                {
                    if (!item.IsValid)
                    {
                        foreach (DbValidationError error in item.ValidationErrors)
                        {
                            errList.Add(error.PropertyName + ":" + error.ErrorMessage);
                        }
                    }
                }
                return errList.AsEnumerable();
            }
            catch
            {
                // Do nothing 
                return new List<string>();
            }
        }

        #region IDisposable Members

        private bool disposedValue;
        /// <summary>
        /// Dispose method
        /// </summary>
        /// <param name="disposing"></param>
        protected void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    // dispose managed state here if required             }            
                    // dispose unmanaged objects and set large fields to null     
                    if (_context != null)
                        _context.Dispose();
                } this.disposedValue = true;
            }
        }
        /// <summary>
        /// Dispose method
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}

