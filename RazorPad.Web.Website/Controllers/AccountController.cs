using System;
using System.Web.Mvc;
using System.Web.Security;
using RazorPad.Web.Services;
using RazorPad.Web.Website.Models.Account;

namespace RazorPad.Web.Website.Controllers
{
    public class AccountController : Controller
    {
        internal static Action<string> AuthenticateUserThunk =
            username => FormsAuthentication.SetAuthCookie(username, false);

        internal static Action DeauthenticateUserThunk =
            () => FormsAuthentication.SignOut();


        private readonly IMembershipService _membershipService;
        private readonly IForgotPasswordEmailer _forgotPasswordEmailer;


        public AccountController(IMembershipService membershipService, IForgotPasswordEmailer forgotPasswordEmailer)
        {
            _membershipService = membershipService;
            _forgotPasswordEmailer = forgotPasswordEmailer;
        }


        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View("ForgotPassword", new ForgotPassword());
        }

        [HttpPost]
        public ActionResult ForgotPassword(string email)
        {
            User user;
            var token = _membershipService.GeneratePasswordResetToken(email, out user);

            var model = new ForgotPassword();

            if (user == null)
            {
                model.EmailNotFound = true;
            }
            else
            {
                var resetPasswordUrl = Url.ExternalAction("ResetPassword", "Account", new {token = token});
                _forgotPasswordEmailer.SendEmail(user, resetPasswordUrl);

                model.Email = user.EmailAddress;
                model.EmailSent = true;
            }

            return View("ForgotPassword", model);
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View("Login");
        }

        [HttpPost]
        public ActionResult Login(string username, string password, string returnUrl)
        {
            if (_membershipService.ValidateUser(username, password))
            {
                AuthenticateUserThunk(username);
                return Redirect("~/");
            }

            ViewBag.errorMsg = "Login failed! Make sure you have entered the right user name and password!";

            return View("Login");
        }

        public ActionResult Logout()
        {
            DeauthenticateUserThunk();
            return RedirectToAction("Index", "RazorPad");
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View("Register");
        }

        [HttpPost]
        public ActionResult Register(string username, string password, string email)
        {
            var isValidUsername = _membershipService.ValidateNewUsername(username);

            if(isValidUsername == false)
                ModelState.AddModelError("username", "Invalid user name (user already exists?)");

            if (ModelState.IsValid == false)
                return View("Register");

            var user = new User
            {
                Username = username,
                Password = password,
                EmailAddress = email,
            };

            _membershipService.CreateUser(user);

            return Login(username, password, null);
        }

        [HttpGet]
        public ActionResult ResetPassword(string token)
        {
            var model = new ResetPassword();
            
            User user;

            if (_membershipService.ValidatePasswordResetToken(token, out user))
            {
                model.UserId = user.Id.ToString();
            }
            else
            {
                model.TokenNotFound = true;
                model.TokenExpiredOrInvalid = true;
            }
            
            return View("ResetPassword", model);
        }

        [HttpPost]
        public ActionResult ResetPassword(string userId, string password, string token)
        {
            _membershipService.ResetPassword(userId, password, token);

            return Login(userId, password, null);
        }
    }
}
