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
            var user = _repository.SingleOrDefault<User>(u => u.Id == id);
            _session.Delete(user);

            if(Request.IsAjaxRequest())
                return new HttpStatusCodeResult(204);

            TempData.Add("Message", "Deleted user "+id);

            return RedirectToAction("Index");
        }

        public ActionResult DeleteAll()
        {
            IQueryable<User> users = _repository.Query<User>();

            foreach (var user in users)
            {
                _session.Delete(user);
            }

            if (Request.IsAjaxRequest())
                return new HttpStatusCodeResult(204);

            TempData.Add("Message", "Deleted "+users.Count()+"users");

            return RedirectToAction("Index");
        }

    }
}
