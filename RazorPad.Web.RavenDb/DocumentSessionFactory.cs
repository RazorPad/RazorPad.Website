using Raven.Client;

namespace RazorPad.Web.RavenDb
{
    public class DocumentSessionFactory
    {
        private readonly IDocumentStore _documentStore;

        public DocumentSessionFactory(IDocumentStore documentStore)
        {
            _documentStore = documentStore;
        }

        public IDocumentSession Create()
        {
            return _documentStore.OpenSession();
        }
    }
}