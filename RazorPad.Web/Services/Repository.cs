using System;
using System.Linq;

namespace RazorPad.Web.Services
{
    public abstract class Repository : IRepository
    {
        public virtual void Delete<TEntity>(params long[] entityIds) where TEntity : class, IEntity
        {
            var entities = Query<TEntity>().Where(x => entityIds.Contains(x.Id));
            foreach (var entity in entities)
            {
                Delete(entity);
            }
        }

        public abstract void Delete<TEntity>(TEntity entity) where TEntity : class, IEntity;

        public abstract void Dispose();

        public abstract IQueryable<TEntity> Query<TEntity>() where TEntity : class, IEntity;
        
        public abstract void Save<TEntity>(TEntity instance) where TEntity : class, IEntity;

        public abstract void SaveChanges();

        public virtual TEntity SingleOrDefault<TEntity>(Func<TEntity, bool> predicate) where TEntity : class, IEntity
        {
            return Query<TEntity>().SingleOrDefault(predicate);
        }
    }
}