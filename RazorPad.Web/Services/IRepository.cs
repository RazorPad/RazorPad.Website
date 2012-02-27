using System;
using System.Linq;

namespace RazorPad.Web.Services
{
    public interface IRepository : IUnitOfWork
    {
        TModel Load<TModel>(string id);

        IQueryable<TModel> Query<TModel>();

        void Save<TModel>(TModel instance);

        TModel SingleOrDefault<TModel>(Func<TModel, bool> predicate);
    }
}
