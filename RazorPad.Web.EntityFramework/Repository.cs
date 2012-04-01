using System;
using System.Diagnostics.Contracts;
using System.Linq;

namespace RazorPad.Web.EntityFramework
{
    public class Repository : Services.Repository
    {
        private readonly RazorPadContext _context;
        private readonly bool _isSharedContext;


        public Repository()
            : this(new RazorPadContext(), false)
        {
        }

        public Repository(RazorPadContext context, bool isSharedContext = true)
        {
            Contract.Requires(context != null);

            _context = context;
            _isSharedContext = isSharedContext;
        }


        public override void Delete<TEntity>(TEntity entity)
        {
            _context.Set<TEntity>().Remove(entity);
        }

        public override void Dispose()
        {
            // If this is a shared (or null) context then
            // we're not responsible for disposing it
            if (_isSharedContext || _context == null)
                return;

            _context.Dispose();
        }

        public override IQueryable<TEntity> Query<TEntity>()
        {
            var items = _context.Set<TEntity>();
            return items;
        }

        public override void Save<TEntity>(TEntity instance)
        {
            Contract.Requires(instance != null);

            _context.Set<TEntity>().Add(instance);
            
            if (_isSharedContext == false)
                _context.SaveChanges();
        }

        public override void SaveChanges()
        {
            _context.SaveChanges();
        }

        public override TEntity SingleOrDefault<TEntity>(Func<TEntity, bool> predicate)
        {
            Contract.Requires(predicate != null);

            return Query<TEntity>().SingleOrDefault(predicate);
        }
    }
}