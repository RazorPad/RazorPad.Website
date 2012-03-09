using System;
using System.Web.Mvc;
using RazorPad.Web.Services;
using RazorPad.Web.Website.Areas.Account.Models;

namespace RazorPad.Web.Website.Areas.Account.Controllers
{
    public class PasswordController : Controller
    {
        private readonly IMembershipService _membershipService;
        private readonly IForgotPasswordEmailer _forgotPasswordEmailer;


        public PasswordController(IMembershipService membershipService, IForgotPasswordEmailer forgotPasswordEmailer)
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
        public ActionResult ResetPassword(ResetPasswordRequest request)
        {
            _membershipService.ResetPassword(request.UserId, request.Password, request.Token);

            return Redirect(request.RedirectUrl);
        }
    }
}
