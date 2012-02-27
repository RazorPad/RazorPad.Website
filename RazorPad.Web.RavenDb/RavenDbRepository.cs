using System;
using System.Linq;
using Raven.Client;
using Raven.Client.Embedded;
using RazorPad.Web.Services;

namespace RazorPad.Web.RavenDb
{
    public class RavenDbRepository : IRepository
    {
        private readonly IDocumentSession _session;

        internal static IDocumentStore Instance
        {
            get
            {
                if (_instance == null)
                    lock (_instanceLock)
                    {
                        if (_instance == null)
                        {

                            // TODO: Replace with DI
#if(AppHarbor)
                            _instance = new Raven.Client.Document.DocumentStore { ConnectionStringName = "RavenDB" };
#else
                            _instance = new EmbeddableDocumentStore { ConnectionStringName = "RavenDB" };
#endif

                            _instance.Conventions.IdentityPartsSeparator = "-";
                            _instance.Initialize();
                        }
                    }

                return _instance;
            }
        }
        private static IDocumentStore _instance;
        private static readonly object _instanceLock = new object();


        public RavenDbRepository()
            : this(Instance.OpenSession())
        {
        }

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