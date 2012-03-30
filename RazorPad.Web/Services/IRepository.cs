using System;
using System.Linq;

namespace RazorPad.Web.Services
{
    public interface IRepository : IUnitOfWork
    {
        void Delete<TEntity>(params long[] entityIds) where TEntity : class, IEntity;
        void Delete<TEntity>(TEntity entity) where TEntity : class, IEntity;

        IQueryable<TEntity> Query<TEntity>() where TEntity : class, IEntity;

        void Save<TEntity>(TEntity instance) where TEntity : class, IEntity;

        TEntity SingleOrDefault<TEntity>(Func<TEntity, bool> predicate) where TEntity : class, IEntity;
    }
}
