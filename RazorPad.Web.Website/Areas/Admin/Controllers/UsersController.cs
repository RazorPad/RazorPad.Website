using System.Linq;
using System.Web.Mvc;
using Raven.Client;
using RazorPad.Web.Services;

namespace RazorPad.Web.Website.Areas.Admin.Controllers
{
    public class UsersController : Controller
    {
        private readonly IRepository _repository;
        private readonly IDocumentSession _session;

        public UsersController(IRepository repository, IDocumentSession session)
        {
            _repository = repository;
            _session = session;
        }

        public ActionResult Index()
        {
            var users = _repository.Query<User>();
            return View("Users", users);
        }

        public ActionResult Delete(long id)
        {
            _session.Advanced.DatabaseCommands.Delete("users-"+id, null);

            if(Request.IsAjaxRequest())
                return new HttpStatusCodeResult(204);

            TempData.Add("Message", "Deleted user "+id);

            return RedirectToAction("Index");
        }
    }
}
