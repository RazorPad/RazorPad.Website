using System;
using System.Diagnostics.Contracts;
using System.Web.Mvc;
using System.Web.Security;
using RazorPad.Web.Facebook;

namespace RazorPad.Web.Website.Areas.Account.Controllers
{
    public class FacebookController : Controller
    {
        internal static Action<string> AuthenticateUserThunk =
            username => FormsAuthentication.SetAuthCookie(username, false);


        private readonly FacebookService _facebook;


        public FacebookController()
            : this(new FacebookService())
        {
        }

        public FacebookController(FacebookService facebook)
        {
            _facebook = facebook;
        }


        public ActionResult Authorize(AuthorizeRequest request)
        {
            if (request.Succeeded)
            {
                var user = Authenticate(request.Code);

                if(user != null)
                {
                    AuthenticateUserThunk(user.Email);
                    return Redirect("~/");
                }
            }

            return View("AuthorizationFailed", request.Error_Description);
        }

        private FacebookUser Authenticate(string code)
        {
            Contract.Requires(string.IsNullOrWhiteSpace(code) == false);

            // TODO: Async
            var authToken = _facebook.Authenticate(code);

            if(authToken == null)
                return null;

            // TODO: Find user ID by Facebook User ID
            // TODO: Create new user if none exists

            var user = _facebook.GetUser(authToken);

            return user;
        }

        public ActionResult Login()
        {
            var loginUrl = _facebook.GetLoginUrl();

            return Redirect(loginUrl);
        }
    }
}
