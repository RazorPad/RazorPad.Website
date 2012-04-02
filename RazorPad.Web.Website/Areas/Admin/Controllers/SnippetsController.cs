using System.Linq;
using System.Web.Mvc;
using RazorPad.Web.Services;
using RazorPad.Web.Website.Models;

namespace RazorPad.Web.Website.Areas.Admin.Controllers
{
    public class SnippetsController : Controller
    {
        private readonly IRepository _repository;

        public SnippetsController(IRepository repository)
        {
            _repository = repository;
        }

        public ActionResult Index(int page = 0, int count = 50)
        {
            var snippets = 
                _repository.Query<Snippet>()
                    .OrderByDescending(x => x.DateCreated)
                    .Skip(page * count)
                    .Take(count)
                    .ToArray();
            
            return View("Snippets", snippets.Select(x => new SnippetViewModel(x)));
        }
    }
}
