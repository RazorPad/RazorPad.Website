using System;
using System.Web.Mvc;
using System.Web.Security;
using RazorPad.Web.Services;

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
