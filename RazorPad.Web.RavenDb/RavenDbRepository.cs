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


        public IQueryable<TModel> Query<TModel>(params string[] includePaths)
            where TModel : class
        {
            return _session.Query<TModel>();
        }

        public TModel SingleOrDefault<TModel>(Func<TModel, bool> predicate)
            where TModel : class
        {
            return _session.Query<TModel>().SingleOrDefault(predicate);
        }

        public void Save<TModel>(TModel instance)
            where TModel : class
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