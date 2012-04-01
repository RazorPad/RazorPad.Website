using System;
using System.Linq;
using Raven.Client;
using RazorPad.Web.Services;

namespace RazorPad.Web.RavenDb
{
    public class RavenDbRepository : Repository
    {
        private readonly IDocumentSession _session;


        public RavenDbRepository(IDocumentSession session)
        {
            _session = session;
        }


        public override void Delete<TEntity>(TEntity entity)
        {
            _session.Delete(entity);
        }

        public override IQueryable<TModel> Query<TModel>()
        {
            return _session.Query<TModel>();
        }

        public override TModel SingleOrDefault<TModel>(Func<TModel, bool> predicate)
        {
            return _session.Query<TModel>().SingleOrDefault(predicate);
        }

        public override void Save<TModel>(TModel instance)
        {
            _session.Store(instance);
        }

        public override void SaveChanges()
        {
            _session.SaveChanges();
        }

        public override void Dispose()
        {
            _session.Dispose();
        }
    }
}