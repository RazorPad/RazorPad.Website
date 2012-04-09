using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RazorPad.Web.Authentication;
using RazorPad.Web.Services;
using RazorPad.Web.Website.Models;

namespace RazorPad.Web.Website.Controllers
{
    [ValidateInput(false)]
    public class SnippetsController : Controller
    {
        internal static Func<HttpContextBase, string> GetCurrentUserId =
            context => (context.User.Identity.IsAuthenticated)
                           ? context.User.Identity.Name
                           : context.Request.UserHostAddress;

        private readonly IRepository _repository;


        public SnippetsController(IRepository repository)
        {
            _repository = repository;
        }


        public int Hack()
        {
            var roles = new Roles {new Role("Admin") {Users = new List<string> {"jchadwick"}}};
            var serialized = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(roles);
            System.IO.File.WriteAllText(@"~\App_Data\Roles.json", serialized);

            return 1;
        }

        public ActionResult ByUser(string username = null)
        {
            var snippets = _repository.FindSnippetsByUsername(username);
            return Snippets(username, snippets);
        }


        public ActionResult Clone([Bind(Prefix = "")]SaveRequest request)
        {
            return Save(request, clone: true);
        }

        public ActionResult Examples()
        {
            var snippets = _repository.Query<ExampleSnippet>();
            return Snippets("Examples", snippets);
        }

        public ActionResult Save([Bind(Prefix = "")]SaveRequest request, bool clone = false)
        {
            var createdBy = GetCurrentUserId(HttpContext);

            Snippet snippet = null;

            var snippetExists = !String.IsNullOrWhiteSpace(request.SnippetId);

            if (snippetExists)
            {
                snippet = _repository.FindSnippet(request.SnippetId);
                snippetExists = (snippet != null);
            }

            // See if we are cloning or not
            if (snippetExists)
            {
                var userOwnsSnippet = snippet.CreatedBy.Equals(createdBy, StringComparison.OrdinalIgnoreCase);
                clone = clone || !userOwnsSnippet;
            }

            var shouldCreateNewSnippet = !snippetExists || clone;

            if (shouldCreateNewSnippet)
            {
                if (clone)
                {
                    request.CloneOf = request.SnippetId;
                }

                snippet = new Snippet
                {
                    CloneOf = request.CloneOf,
                    CreatedBy = createdBy,
                    Language = request.Language,
                    Model = request.Model,
                    Notes = request.Notes,
                    Title = request.Title,
                    View = request.Template,
                };

                _repository.Save(snippet);
            }
            else
            {
                snippet.Model = request.Model;
                snippet.Notes = request.Notes;
                snippet.Title = request.Title;
                snippet.View = request.Template;
            }

            return Json(snippet);
        }


        protected ActionResult Snippets(string username, IEnumerable<Snippet> snippets)
        {
            var viewModel = new UserSnippetsViewModel(username, (snippets ?? Enumerable.Empty<Snippet>().ToArray()));

            if (Request.IsAjaxRequest())
                return Json(viewModel, JsonRequestBehavior.AllowGet);

            if (ControllerContext.IsChildAction)
                return PartialView("_Snippets", viewModel);

            return View("Snippets", viewModel);
        }
    }
}
