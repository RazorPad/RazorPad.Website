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
            instance = new EmbeddableDocumentStore { ConnectionStringName = "RazorPadDB" };
            instance.Conventions.IdentityPartsSeparator = "-";
            instance.Initialize();
            return instance;
        }
    }
}