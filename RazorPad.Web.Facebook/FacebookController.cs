using System.Diagnostics.Contracts;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace RazorPad.Web.Facebook
{
    public class FacebookController : Controller
    {
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

                if(user == null)
                    return View();

                FormsAuthentication.SetAuthCookie(user, true);

                return Redirect("~/");
            }

            if (request.DeniedByUser)
                return View("AuthorizationDenied");

            return View("AuthorizationFailed", request.Error_Description);
        }

        private string Authenticate(string code)
        {
            Contract.Requires(string.IsNullOrWhiteSpace(code) == false);

            var redirectUrl = Request.ExternalUrl(VirtualPathUtility.ToAbsolute("~/"));
            
            // TODO: Async
            var authToken = _facebook.Authenticate(code, redirectUrl);

            if(authToken == null)
                return null;

            // TODO: Find user ID by Facebook User ID
            // TODO: Create new user if none exists

            var user = _facebook.GetUser(authToken);

            return user.UserId.ToString();
        }

        public ActionResult Login()
        {
            var authorizeUrl = Url.ExternalAction("Authorize", "Facebook");
            var loginUrl = _facebook.GetLoginUrl(authorizeUrl);

            return Redirect(loginUrl);
        }
    }
}
