using System;
using System.Data.Entity.Infrastructure;
using System.Diagnostics.Contracts;
using System.Linq;
using RazorPad.Web.Services;

namespace RazorPad.Web.EntityFramework
{
    public class Repository : IRepository
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


        public void Dispose()
        {
            // If this is a shared (or null) context then
            // we're not responsible for disposing it
            if (_isSharedContext || _context == null)
                return;

            _context.Dispose();
        }

        public IQueryable<TModel> Query<TModel>(params string[] includePaths)
            where TModel : class
        {
            DbQuery<TModel> items = _context.Set<TModel>();

            foreach (var path in includePaths ?? Enumerable.Empty<string>())
            {
                items = items.Include(path);
            }

            return items;
        }

        public void Save<TModel>(TModel instance)
            where TModel : class
        {
            Contract.Requires(instance != null);

            _context.Set<TModel>().Add(instance);
            
            if (_isSharedContext == false)
                _context.SaveChanges();
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public TModel SingleOrDefault<TModel>(Func<TModel, bool> predicate)
            where TModel : class
        {
            Contract.Requires(predicate != null);

            return Query<TModel>().SingleOrDefault(predicate);
        }
    }
}