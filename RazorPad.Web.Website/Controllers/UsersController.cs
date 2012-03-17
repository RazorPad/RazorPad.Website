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

        public ActionResult Fiddles(string username = null)
        {
            if (username == null)
                return List();

            var fiddles = _repository.FindFiddlesByUsername(username) ?? Enumerable.Empty<Fiddle>();

            var viewModel = new UserFiddlesViewModel
                                {
                                    Fiddles = fiddles.Select(x => new FiddleViewModel(x)),
                                    Username = username,
                                };

            if (Request.IsAjaxRequest())
                return Json(viewModel, JsonRequestBehavior.AllowGet);

            return View("Fiddles", viewModel);
        }


        private ActionResult List()
        {
            // TODO: paging
            var users = _repository.Query<User>();
            return View("Users", users);
        }
    }
}
