using System;
using System.Web.Mvc;
using System.Web.Security;
using System.Text;
using System.Net.Mail;
using System.Net;
using System.Configuration;
using RazorPad.Web.Services;

namespace RazorPad.Web.Website.Controllers
{
    public class AccountController : Controller
    {
        private readonly IRepository _repository;

        public AccountController(IRepository repository)
        {
            _repository = repository;
        }

        public ActionResult Login(string userName, string password, string returnUrl)
        {
            if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password))
            {
                if (IsValidUser(userName, password))
                {
                    FormsAuthentication.SetAuthCookie(userName, false);
                    return Redirect("~/");
                }

                ViewBag.errorMsg = "Login failed! Make sure you have entered the right user name and password!";
            }

            return View("Login");
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "RazorPad");
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View("Register");
        }

        [HttpPost]
        public ActionResult Register(string userName, string password, string email)
        {
            //ToDo: Check if UserName already exist

            if (ModelState.IsValid == false)
                return View("Register");

            var user = new User
            {
                UserName = userName,
                Password = password,
                Email = email,
                DateCreated = DateTime.UtcNow
            };

            _repository.Save(user);
            _repository.SaveChanges();

            return Login(userName, password, null);
        }

        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View("ForgotPassword", new ForgotPassword());
        }

        [HttpPost]
        public ActionResult ForgotPassword(string email)
        {
            var user = _repository.SingleOrDefault<User>(u => u.Email == email);

            var model = new ForgotPassword();

            if (user == null)
            {
                model.EmailNotFound = true;
            }
            else
            {
                user.ForgotPasswordToken = Guid.NewGuid().ToString();
                _repository.SaveChanges();

                //ToDo: Send the mail
                var sbMailMsg = new StringBuilder();
                sbMailMsg.AppendFormat("Hi {0},<br /><br />", user.UserName);
                sbMailMsg.Append("Please click the below link to reset your password.<br /><br />");
                sbMailMsg.AppendFormat("<a href=\"{0}\">{0}</a>",
                                       Url.ExternalAction("ResetPassword", "Account",
                                                          new {token = user.ForgotPasswordToken}));
                sbMailMsg.Append("<br /><br />- RazorPad");

                var mailMessage = new MailMessage();
                mailMessage.To.Add(email);
                mailMessage.Subject = "RazorPad - Password Reset";
                mailMessage.Body = sbMailMsg.ToString();
                mailMessage.IsBodyHtml = true;

                var smtpClient = new SmtpClient
                                     {
                                         Credentials =
                                             new NetworkCredential(
                                             ConfigurationManager.AppSettings["SmtpClient.Username"],
                                             ConfigurationManager.AppSettings["SmtpClient.Password"])
                                     };
                smtpClient.Send(mailMessage);

                model.Email = user.Email;
                model.EmailSent = true;
            }

            return View("ForgotPassword", model);
        }

        [HttpGet]
        public ActionResult ResetPassword(string token)
        {
            var model = new ResetPassword();
            model.TokenNotFound = string.IsNullOrEmpty(token);
            if(!model.TokenNotFound)
            {
                var user = _repository.SingleOrDefault<User>(u => u.ForgotPasswordToken == token);
                
                model.TokenExpiredOrInvalid = (user == null);
                if(!model.TokenExpiredOrInvalid)
                {
                    model.UserId = user.Id.ToString();
                }
            }
            
            return View("ResetPassword", model);
        }

        [HttpPost]
        public ActionResult ResetPassword(string userId, string password)
        {
            // TODO: FIX MAJOR SECURITY HOLE -- VALIDATE THE TOKEN!
            var user = _repository.SingleOrDefault<User>(u => u.UserName == userId);

            if(user != null)
            {
                user.Password = password;
                _repository.SaveChanges();
            }

            return RedirectToAction("Login", "Account");
        }

        private bool IsValidUser(string userName, string password)
        {
            var user = _repository.SingleOrDefault<User>(u => u.UserName == userName && u.Password == password);
            return user != null;
        }
    }
}
