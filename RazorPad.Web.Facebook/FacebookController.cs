﻿using System.Diagnostics.Contracts;
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
                    return AuthenticationFailed();

                FormsAuthentication.SetAuthCookie(user, true);

                return Redirect("~/");
            }

            if (request.DeniedByUser)
                return AuthorizationDenied();

            return AuthorizationFailed(request.Error_Description);
        }

        private ActionResult AuthenticationFailed()
        {
            return View();
        }

        private string Authenticate(string code)
        {
            Contract.Requires(string.IsNullOrWhiteSpace(code) == false);

            var redirectUrl = Request.ExternalUrl(VirtualPathUtility.ToAbsolute("~/"));
            
            // TODO: Async
            var facebookUser = _facebook.Authenticate(code, redirectUrl);

            if(facebookUser == null)
                return null;

            // TODO: Find user ID by Facebook User ID
            // TODO: Create new user if none exists

            return facebookUser.EmailAddress;
        }

        private ActionResult AuthorizationFailed(string reason)
        {
            return View("AuthorizationFailed", reason);
        }

        private ActionResult AuthorizationDenied()
        {
            return View("AuthorizationDenied");
        }
    }
}
