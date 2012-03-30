using System.Web.Mvc;
using RazorPad.Web.Services;

namespace RazorPad.Web.Website.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserManagementController : Controller
    {
        private readonly IRepository _repository;

        public UserManagementController(IRepository repository)
        {
            _repository = repository;
        }

        public ActionResult Index()
        {
            var users = _repository.Query<User>();
            return View("Users", users);
        }

        public ActionResult Delete(long id)
        {
            _repository.Delete<User>(id);

            if(Request.IsAjaxRequest())
                return new HttpStatusCodeResult(204);

            TempData.Add("Message", "Deleted user "+id);

            return RedirectToAction("Index");
        }
    }
}
