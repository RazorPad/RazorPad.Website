using System;
using System.Diagnostics.Contracts;
using System.Web.Mvc;
using RazorPad.Web.Authentication;
using RazorPad.Web.Authentication.Facebook;
using RazorPad.Web.Services;
using RazorPad.Web.Website.Areas.Account.Models;

namespace RazorPad.Web.Website.Areas.Account.Controllers
{
    public class FacebookController : Controller
    {
        private readonly FacebookService _facebook;
        private readonly IRepository _repository;


        public FacebookController(FacebookService facebook, IRepository repository)
        {
            _facebook = facebook;
            _repository = repository;
        }


        public ActionResult Authorize(AuthorizeRequest request)
        {
            if (request.Succeeded)
                return Authenticate(request.Code);

            return View("AuthorizationFailed", request.Error_Description);
        }

        public ActionResult Login()
        {
            var loginUrl = _facebook.GetLoginUrl();

            return Redirect(loginUrl);
        }

        private ActionResult Authenticate(string code)
        {
            Contract.Requires(string.IsNullOrWhiteSpace(code) == false);

            var authToken = _facebook.Authenticate(code);

            if(authToken == null)
                return null;

            Func<IntegratedAuthenticationCredential, bool> matchesAuthToken =
                x => x.Token == authToken.Token;

            IntegratedAuthenticationCredential credential = null;

            var user = _repository.FindUserByCredential(matchesAuthToken);

            if (user != null)
            {
                credential = user.GetCredential(matchesAuthToken);
            }
            else
            {
                var facebookUser = _facebook.GetUser(authToken);

                if (facebookUser != null && !string.IsNullOrWhiteSpace(facebookUser.Email))
                {
                    var facebookUserId = facebookUser.Id.ToString();

                    user = _repository.FindUserByEmail(facebookUser.Email);

                    if(user != null)
                    {
                        credential = user.GetCredential<IntegratedAuthenticationCredential>(
                            x => x.UserId == facebookUserId);

                        if (credential == null)
                        {
                            credential = new IntegratedAuthenticationCredential { UserId = facebookUserId };
                            user.Credentials.Add(credential);
                        }
                    }
                }

                if (user == null)
                    return RegisterNewUser(facebookUser, authToken.Token);
            }

            credential.Expiration = authToken.Expiration;
            credential.Token = authToken.Token;

            FormsAuthController.AuthenticateUser(user.EmailAddress);

            return Redirect("~/");
        }

        private ActionResult RegisterNewUser(FacebookUser user, string token)
        {
            var routeValues = new {
                area = "Account",
                EmailAddress = user.Email,
                Token = token,
                UserId = user.Id,
                Username = user.Name,
            };

            return RedirectToAction("Integrated", "Registration", routeValues);
        }
    }
}
