using System.Linq;
using System.Web.Mvc;
using RazorPad.Web.Services;
using RazorPad.Web.Website.Models;

namespace RazorPad.Web.Website.Controllers
{
    public class UsersController : Controller
    {
        private readonly IRepository _repository;

        public UsersController(IRepository repository)
        {
            _repository = repository;
        }

        public ActionResult Index(string username = null)
        {
            if (username == null)
                return List();

            var user = _repository.FindUserByUsername(username);

            if (user == null)
                return View("UserNotFound", username);

            return View("User", user);
        }

        public ActionResult Snippets(string username = null)
        {
            if (username == null)
                return List();

            var viewModel = GetUserSnippetsViewModel(username);

            if (Request.IsAjaxRequest())
                return Json(viewModel, JsonRequestBehavior.AllowGet);

            return View("Snippets", viewModel);
        }

        private UserSnippetsViewModel GetUserSnippetsViewModel(string username)
        {
            var snippets = _repository.FindSnippetsByUsername(username) ?? Enumerable.Empty<Snippet>();

            var viewModel = new UserSnippetsViewModel
                                {
                                    Snippets = snippets.Select(x => new SnippetViewModel(x)),
                                    Username = username,
                                };
            return viewModel;
        }

        [ChildActionOnly]
        public ActionResult UserSnippets(string username)
        {
            var viewModel = GetUserSnippetsViewModel(username);
            return PartialView("_Snippets", viewModel);
        }


        private ActionResult List()
        {
            // TODO: paging
            var users = _repository.Query<User>();
            return View("Users", users);
        }
    }
}
