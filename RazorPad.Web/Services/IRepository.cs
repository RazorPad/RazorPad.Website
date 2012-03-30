using System;
using System.Linq;

namespace RazorPad.Web.Services
{
    public interface IRepository : IUnitOfWork
    {
        IQueryable<TModel> Query<TModel>(params string[] includePaths) where TModel : class;

        void Save<TModel>(TModel instance) where TModel : class;

        TModel SingleOrDefault<TModel>(Func<TModel, bool> predicate) where TModel : class;
    }
}
