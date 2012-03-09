using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using RazorPad.Web.Services;
using RazorPad.Web.Website.Areas.Account.Models;

namespace RazorPad.Web.Website.Areas.Account.Controllers
{
    public class FormsAuthController : Controller
    {
        internal static Action<string> AuthenticateUserThunk =
            username => FormsAuthentication.SetAuthCookie(username, false);

        internal static Action DeauthenticateUserThunk =
            () => FormsAuthentication.SignOut();


        private readonly IMembershipService _membershipService;


        public FormsAuthController(IMembershipService membershipService)
        {
            _membershipService = membershipService;
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View("Login");
        }

        [HttpPost]
        public ActionResult Login(LoginRequest loginRequest)
        {
            if (_membershipService.ValidateUser(loginRequest.Username, loginRequest.Password))
            {
                AuthenticateUserThunk(loginRequest.Username);
                return Redirect("~/");
            }

            ModelState.AddModelError(string.Empty, "Invalid username and password");

            return View("Login");
        }

        public ActionResult Logout(string redirectUrl = null)
        {
            DeauthenticateUserThunk();

            if(redirectUrl != null)
                return Redirect(HttpUtility.UrlDecode(redirectUrl));

            return Redirect("~/");
        }


        internal static void AuthenticateUser(User user)
        {
            AuthenticateUser(user.Username);
        }

        internal static void AuthenticateUser(string username)
        {
            AuthenticateUserThunk(username);
        }
    }
}