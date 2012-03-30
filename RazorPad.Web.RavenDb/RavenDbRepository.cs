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


        public override void Delete<TModel>(params long[] entityIds)
        {
            foreach(var entityId in entityIds)
            {
                var key = string.Format("{0}s-{1}", typeof(TModel).Name.ToLower(), entityId);
                _session.Advanced.DatabaseCommands.Delete(key, null);
            }
        }

        public override void Delete<TModel>(TModel entity)
        {
            _session.Delete(entity);
        }

        public override IQueryable<TModel> Query<TModel>()
        {
            return _session.Query<TModel>();
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
