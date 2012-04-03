using System;
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

        public ActionResult Index()
        {
            return Snippets();
        }

        public ActionResult Snippets(string username = null, int page = 0, int count = 50)
        {
            ViewBag.Page = page;
            ViewBag.Count = count;
            ViewBag.Username = username;

            var snippets = 
                _repository.Query<Snippet>()
                    .OrderByDescending(x => x.DateCreated)
                    .Skip(page * count)
                    .Take(count);

            if (!string.IsNullOrWhiteSpace(username))
                snippets = snippets.Where(x => x.CreatedBy.Equals(username, StringComparison.OrdinalIgnoreCase));

            return View("Snippets", snippets.ToArray().Select(x => new SnippetViewModel(x)));
        }
    }
}
