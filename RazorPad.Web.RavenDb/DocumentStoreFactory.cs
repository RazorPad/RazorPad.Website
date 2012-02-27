using Raven.Client;

namespace RazorPad.Web.RavenDb
{
    public class DocumentStoreFactory
    {
        public static void Initialize(IDocumentStore store)
        {
            store.Conventions.IdentityPartsSeparator = "-";
            store.Initialize();
        }
    }
}