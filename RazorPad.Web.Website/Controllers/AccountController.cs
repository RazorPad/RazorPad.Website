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
                ModelState.AddModelError("EmailAddress", "Email address not found.");
            }
            else
            {
                model.EmailAddress = user.EmailAddress;

                var resetPasswordUrl = Url.ExternalAction("ResetPassword", "Account", new { token = token });

                try
                {
                    _forgotPasswordEmailer.SendEmail(user, resetPasswordUrl);
                    return View("ForgotPasswordEmailSent", model);
                }
                catch (Exception)
                {
                    ModelState.AddModelError("EmailAddress", "Error sending email.  Please try again.");
                }

#if(DEBUG)                
                model.Token = token;
#endif
            }

            return View("ForgotPassword", model);
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

        public ActionResult Logout()
        {
            DeauthenticateUserThunk();
            return RedirectToAction("Index", "RazorPad");
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View("Register", new CreateNewUserRequest());
        }

        [HttpPost]
        public ActionResult Register(CreateNewUserRequest createNewUserRequest)
        {
            var isValidUsername = _membershipService.ValidateNewUsername(createNewUserRequest.Username);

            if(isValidUsername == false)
                ModelState.AddModelError("username", "Invalid user name (user already exists?)");

            if (ModelState.IsValid == false)
            {
                createNewUserRequest.Password = null;
                createNewUserRequest.PasswordConfirm = null;
                return View("Register", createNewUserRequest);
            }

            var user = new User
            {
                Username = createNewUserRequest.Username,
                EmailAddress = createNewUserRequest.Email,
            };

            user.Credentials.Add(FormsAuthCredential.Create(createNewUserRequest.Password));

            _membershipService.CreateUser(user);

            return Login(new LoginRequest(createNewUserRequest.Username, createNewUserRequest.Password));
        }

        [HttpGet]
        public ActionResult ResetPassword(string token = null)
        {
            var model = new ResetPasswordRequest();

            User user;
            if (!string.IsNullOrWhiteSpace(token) && _membershipService.ValidatePasswordResetToken(token, out user))
            {
                model.UserId = user.Id.ToString();
            }
            else
            {
                return View("InvalidResetPasswordToken");
            }
            
            return View("ResetPassword", model);
        }

        [HttpPost]
        public ActionResult ResetPassword(string userId, string password, string token)
        {
            _membershipService.ResetPassword(userId, password, token);

            return Login(new LoginRequest(userId, password));
        }
    }
}
