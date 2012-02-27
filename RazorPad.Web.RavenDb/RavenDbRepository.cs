using System;
using System.Linq;
using Raven.Client;
using RazorPad.Web.Services;

namespace RazorPad.Web.RavenDb
{
    public class RavenDbRepository : IRepository
    {
        private readonly IDocumentSession _session;


        public RavenDbRepository(IDocumentSession session)
        {
            _session = session;
        }


        public TModel Load<TModel>(string id)
        {
            return _session.Load<TModel>(id);
        }

        public IQueryable<TModel> Query<TModel>()
        {
            return _session.Query<TModel>();
        }

        public TModel SingleOrDefault<TModel>(Func<TModel, bool> predicate)
        {
            return _session.Query<TModel>().SingleOrDefault(predicate);
        }

        public void Save<TModel>(TModel instance)
        {
            _session.Store(instance);
        }

        public void SaveChanges()
        {
            _session.SaveChanges();
        }

        public void Dispose()
        {
            _session.Dispose();
        }
    }
}