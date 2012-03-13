using System.Web.Mvc;

namespace RazorPad.Web.Website.Areas.Account.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View("Index");
        }
    }
}
