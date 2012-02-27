using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RazorPad.Website.Models;
using System.Web.Security;
using Raven.Abstractions.Data;
using System.Text;
using RazorPad.Core;
using System.Net.Mail;
using System.Net;
using System.Configuration;

namespace RazorPad.Website.Controllers
{
    public class AccountController : Controller
    {
        #region ---- Actions ----

        #region ---- Login ----
        public ActionResult Login(string UserName, string Password, string ReturnUrl)
        {
            if (!string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(Password))
            {
                if (ValidateUser(UserName, Password))
                    RedirectFromLoginPage(UserName, ReturnUrl);
                else
                    ViewBag.errorMsg = "Login failed! Make sure you have entered the right user name and password!";
            }

            return View("Login");
        }
        #endregion

        #region ---- Logout ----
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "RazorPad");
        }
        #endregion

        #region ---- Register ----
        [HttpGet]
        public ActionResult Register()
        {
            return View("Register");
        }

        [HttpPost]
        public ActionResult Register(string UserName, string Password, string Email)
        {
            //ToDo: Check if UserName already exist

            if (ModelState.IsValid == false)
                return View("Register");

            var session = DataDocumentStore.Instance.OpenSession();
            var userInfo = new User
            {
                UserName = UserName,
                Password = Password,
                Email = Email,
                DateCreated = DateTime.UtcNow
            };
            session.Store(userInfo);
            session.SaveChanges();
            session.Dispose();

            return Login(UserName, Password, null);
        }
        #endregion

        #region ---- ForgotPassword ----
        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View("ForgotPassword", new ForgotPassword());
        }

        [HttpPost]
        public ActionResult ForgotPassword(string Email)
        {
            var session = DataDocumentStore.Instance.OpenSession();
            var userInfo = session.Query<User>()
                            .Where(u => u.Email == Email)
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
                sbMailMsg.AppendFormat("<a href=\"{0}\">{0}</a>", Url.AbsoluteAction("ResetPassword", "Account", null) + "?token=" + token);
                sbMailMsg.Append("<br /><br />- RazorPad");

                var mailMessage = new MailMessage();
                mailMessage.To.Add(Email);
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
        #endregion

        #region ---- ResetPassword ----
        [HttpGet]
        public ActionResult ResetPassword(string Token)
        {
            var model = new ResetPassword();
            model.TokenNotFound = string.IsNullOrEmpty(Token);
            if(!model.TokenNotFound)
            {
                var session = DataDocumentStore.Instance.OpenSession();
                var userInfo = session.Query<User>()
                                .Where(u => u.ForgotPasswordToken == Token)
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
        public ActionResult ResetPassword(string UserId, string Password)
        {
            var session = DataDocumentStore.Instance.OpenSession();
            session.Advanced.DatabaseCommands.Patch(
                    "Users-" + UserId,
                    new[]
                        {
                            new PatchRequest
                                {
                                    Type = PatchCommandType.Set,
                                    Name = "Password",
                                    Value = Password
                                }
                        });
            session.SaveChanges();
            session.Dispose();

            return RedirectToAction("Login", "Account");
        }
        #endregion

        #endregion

        #region ---- Private Methods ----

        private bool ValidateUser(string UserName, string Password)
        {
            bool isValid = false;
            var session = DataDocumentStore.Instance.OpenSession();
            var userInfo = session.Query<User>()
                           .Where(u => u.UserName == UserName && u.Password == Password)
                           .ToArray<User>();
            if (userInfo != null && userInfo.Length > 0)
            {
                isValid = string.Compare(userInfo[0].Password, Password, true) == 0;
            }
            session.Dispose();
            return isValid;
        }

        private void RedirectFromLoginPage(string UserName, string ReturnUrl)
        {
            //ToDo: Persist User
            FormsAuthentication.SetAuthCookie(UserName, false);

            if (!string.IsNullOrEmpty(ReturnUrl))
                Response.Redirect(ReturnUrl);
            else
                Response.Redirect(FormsAuthentication.DefaultUrl);
        }
        #endregion
    }
}
