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


        public void Delete<TModel>(long entityId) where TModel : class
        {
            var key = string.Format("{0}s-{1}", typeof (TModel).Name.ToLower(), entityId);
            _session.Advanced.DatabaseCommands.Delete(key, null);
        }

        public void Delete<TModel>(TModel entity) where TModel : class
        {
            _session.Delete(entity);
        }

        public IQueryable<TModel> Query<TModel>() where TModel : class
        {
            return _session.Query<TModel>();
        }

        public TModel SingleOrDefault<TModel>(Func<TModel, bool> predicate) where TModel : class
        {
            return _session.Query<TModel>().SingleOrDefault(predicate);
        }

        public void Save<TModel>(TModel instance) where TModel : class
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