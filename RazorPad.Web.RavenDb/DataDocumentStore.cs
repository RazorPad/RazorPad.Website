using System;
using Raven.Client;
using Raven.Client.Embedded;

namespace RazorPad.Website.Models
{
    public class DataDocumentStore
    {
        internal static IDocumentStore Instance
        {
            get 
            {
                if(_instance == null)
                    throw new InvalidOperationException("IDocumentStore has not been initialized.");
                return _instance;
            }
        }
        private static IDocumentStore _instance;

        public static IDocumentSession OpenSession()
        {
            return Instance.OpenSession();
        }

        public static IDocumentStore Initialize()
        {
// TODO: Replace with DI
#if(AppHarbor)
            instance = new Raven.Client.Document.DocumentStore { ConnectionStringName = "RavenDB" };
#else
            _instance = new EmbeddableDocumentStore { ConnectionStringName = "RavenDB" };
#endif

            _instance.Conventions.IdentityPartsSeparator = "-";
            _instance.Initialize();
            return _instance;
        }
    }
}