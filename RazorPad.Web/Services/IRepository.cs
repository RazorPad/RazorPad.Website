using System;
using System.Linq;

namespace RazorPad.Web.Services
{
    public interface IRepository : IUnitOfWork
    {
        void Delete<TModel>(long entityId) where TModel : class;
        void Delete<TModel>(TModel entity) where TModel : class;

        IQueryable<TModel> Query<TModel>() where TModel : class;

        void Save<TModel>(TModel instance) where TModel : class;

        TModel SingleOrDefault<TModel>(Func<TModel, bool> predicate) where TModel : class;
    }
}
