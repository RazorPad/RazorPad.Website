using System;
using Raven.Client;
using Raven.Client.Embedded;

namespace RazorPad.Website.Models
{
    public class DataDocumentStore
    {
        private static IDocumentStore instance;

        public static IDocumentStore Instance
        {
            get 
            {
                if(instance == null)
                    throw new InvalidOperationException("IDocumentStore has not been initialized.");
                return instance;
            }
        }

        public static IDocumentStore Initialize()
        {
#if(DEBUG)
            instance = new EmbeddableDocumentStore { ConnectionStringName = "RavenDB" };
#else
            instance = new Raven.Client.Document.DocumentStore { ConnectionStringName = "RavenDB" };
#endif

            instance.Conventions.IdentityPartsSeparator = "-";
            instance.Initialize();
            return instance;
        }
    }
}