using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using Raven.Abstractions.Data;
using System.Text;
using System.Net.Mail;
using System.Net;
using System.Configuration;
using Raven.Client.Linq;
using RazorPad.Core;
using RazorPad.Web.RavenDb;

namespace RazorPad.Web.Website.Controllers
{
    public class AccountController : Controller
    {
        public ActionResult Login(string userName, string password, string returnUrl)
        {
            if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password))
            {
                if (ValidateUser(userName, password))
                {
                    FormsAuthentication.SetAuthCookie(userName, false);
                    return Redirect("~/");
                }
                else
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

            var session = DataDocumentStore.OpenSession();
            var userInfo = new User
            {
                UserName = userName,
                Password = password,
                Email = email,
                DateCreated = DateTime.UtcNow
            };
            session.Store(userInfo);
            session.SaveChanges();
            session.Dispose();

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
            var session = DataDocumentStore.OpenSession();
            var userInfo = session.Query<User>()
                            .Where(u => u.Email == email)
                            .ToArray<User>();
            var model = new ForgotPassword();
            model.EmailNotFound = (userInfo == null || userInfo.Length == 0);
            if(!model.EmailNotFound) 
            {
                //ToDo: Generate expirable excrypted token
                string token = Guid.NewGuid().ToString();
                session.Advanced.DatabaseCommands.Patch(
                    "Users-" + userInfo[0].Id.ToString(),
                    new[]
                        {
                            new PatchRequest
                                {
                                    Type = PatchCommandType.Set,
                                    Name = "ForgotPasswordToken",
                                    Value = token
                                }
                        });
                session.SaveChanges();

                //ToDo: Send the mail
                var sbMailMsg = new StringBuilder();
                sbMailMsg.AppendFormat("Hi {0},<br /><br />", userInfo[0].UserName);
                sbMailMsg.Append("Please click the below link to reset your password.<br /><br />");
                sbMailMsg.AppendFormat("<a href=\"{0}\">{0}</a>", Url.ExternalAction("ResetPassword", "Account", null) + "?token=" + token);
                sbMailMsg.Append("<br /><br />- RazorPad");

                var mailMessage = new MailMessage();
                mailMessage.To.Add(email);
                mailMessage.Subject = "RazorPad - Password Reset";
                mailMessage.Body = sbMailMsg.ToString();
                mailMessage.IsBodyHtml = true;

                var smtpClient = new SmtpClient { 
                    Credentials = new NetworkCredential(ConfigurationManager.AppSettings["SmtpClient.Username"],
                                                        ConfigurationManager.AppSettings["SmtpClient.Password"])
                };
                smtpClient.Send(mailMessage);

                model.Email = userInfo[0].Email;
                model.EmailSent = true;
            }
            session.Dispose();
            
            return View("ForgotPassword", model);
        }

        [HttpGet]
        public ActionResult ResetPassword(string token)
        {
            var model = new ResetPassword();
            model.TokenNotFound = string.IsNullOrEmpty(token);
            if(!model.TokenNotFound)
            {
                var session = DataDocumentStore.OpenSession();
                var userInfo = session.Query<User>()
                                .Where(u => u.ForgotPasswordToken == token)
                                .ToArray<User>();
                
                //ToDo: Check if the token is expired
                var isTokenExpired = false;
                model.TokenExpiredOrInvalid = ((userInfo != null && userInfo.Length == 0) || isTokenExpired);
                if(!model.TokenExpiredOrInvalid)
                {
                    model.UserId = userInfo[0].Id.ToString();
                }
                session.Dispose();
            }
            
            return View("ResetPassword", model);
        }

        [HttpPost]
        public ActionResult ResetPassword(string userId, string password)
        {
            var session = DataDocumentStore.OpenSession();
            session.Advanced.DatabaseCommands.Patch(
                    "Users-" + userId,
                    new[]
                        {
                            new PatchRequest
                                {
                                    Type = PatchCommandType.Set,
                                    Name = "Password",
                                    Value = password
                                }
                        });
            session.SaveChanges();
            session.Dispose();

            return RedirectToAction("Login", "Account");
        }

        private bool ValidateUser(string userName, string password)
        {
            bool isValid = false;
            var session = DataDocumentStore.OpenSession();
            var userInfo = session.Query<User>()
                           .Where(u => u.UserName == userName && u.Password == password)
                           .ToArray();
            if (userInfo != null && userInfo.Length > 0)
            {
                isValid = string.Compare(userInfo[0].Password, password, true) == 0;
            }
            session.Dispose();
            return isValid;
        }
    }
}
