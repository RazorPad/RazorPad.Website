using System;
using System.Collections.Generic;

namespace RazorPad.Web.Services
{
    public interface IRepository : IUnitOfWork
    {
        TModel Load<TModel>(string id);

        IEnumerable<TModel> Query<TModel>(Func<TModel, bool> predicate = null);

        TModel SingleOrDefault<TModel>(Func<TModel, bool> predicate);
    }
}
