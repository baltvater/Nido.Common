using RefactorThis.GraphDiff;
using System;
namespace Nido.Common.BackEnd
{
    interface IHandlerBase<E, C>
        where E : BaseObject, new()
        where C : BaseObjectConext, IBaseObjectConext, new()
    {
        GenericResponse<E> AddBulkGeneric(System.Collections.Generic.IEnumerable<E> entities);
        GenericResponse<E> AddGeneric(E entity);
        HandlerBase<E, C> AttachChild<OtherE>(OtherE entity) where OtherE : BaseObject;
        int CountGeneric();
        int CountGeneric(System.Linq.Expressions.Expression<Func<E, bool>> predicate);
        GenericResponse<E> DeleteGeneric(E entity);
        GenericResponse<E> DeleteGeneric(int entityId);
        GenericResponse<E> DeleteGeneric(System.Linq.Expressions.Expression<Func<E, bool>> filterExpression);
        void Dispose();
        HandlerBase<E, C> FromCache();
        HandlerBase<E, C> FromCache(EntityFramework.Caching.CachePolicy cachePolicy = null, System.Collections.Generic.IEnumerable<string> tags = null);
        HandlerBase<E, C> FromCacheFirstOrDefault(EntityFramework.Caching.CachePolicy cachePolicy = null, System.Collections.Generic.IEnumerable<string> tags = null);
        GenericResponse<System.Collections.Generic.IEnumerable<E>> GetAllGeneric();
        GenericResponse<System.Collections.Generic.IEnumerable<E>> GetAllGeneric(System.Linq.Expressions.Expression<Func<E, bool>> predicate);
        GenericResponse<System.Collections.Generic.IEnumerable<E>> GetAllGenericFromCache(EntityFramework.Caching.CachePolicy cachePolicy = null, System.Collections.Generic.IEnumerable<string> tags = null);
        GenericResponse<System.Collections.Generic.IEnumerable<E>> GetAllGenericFromCache(System.Linq.Expressions.Expression<Func<E, bool>> predicate, EntityFramework.Caching.CachePolicy cachePolicy = null, System.Collections.Generic.IEnumerable<string> tags = null);
        GenericResponse<E> GetFirstGeneric(System.Linq.Expressions.Expression<Func<E, bool>> predicate);
        GenericResponse<E> GetSingleGeneric(System.Linq.Expressions.Expression<Func<E, bool>> predicate);
        HandlerBase<E, C> Include(System.Linq.Expressions.Expression<Func<E, object>> path);
        HandlerBase<E, C> Include(string entityInclude);
        GenericResponse<E> UpdateGeneric(E entity);
        GenericResponse<E> UpdateGeneric(E entity, int entityId);
        GenericResponse<E> UpdateGeneric(System.Linq.Expressions.Expression<Func<E, E>> updateExpression);
        GenericResponse<E> UpdateGeneric(System.Linq.Expressions.Expression<Func<E, bool>> filterExpression, System.Linq.Expressions.Expression<Func<E, E>> updateExpression);
        GenericResponse<E> UpdateGraphGeneric(E entity, System.Linq.Expressions.Expression<Func<IUpdateConfiguration<E>, object>> mapping);
    }
}
