using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RazorPad.Website.Models;
using System.Web.Security;   

namespace RazorPad.Website.Controllers
{
    public class AccountController : Controller
    {
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

        public ActionResult Register(string UserName, string Password, string Email)
        {
            if (!string.IsNullOrEmpty(UserName)
                && !string.IsNullOrEmpty(Password)
                && !string.IsNullOrEmpty(Email))
            {
                var session = DataDocumentStore.Instance.OpenSession();
                //ToDo: Check if UserName already exist
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

                Response.Redirect(Url.Action("Login", "Account"));
                
            }

            return View("Register");
        }

        private bool ValidateUser(string UserName, string Password)
        {
            bool isValid = false;
            var session = DataDocumentStore.Instance.OpenSession();
            var userInfo = session.Query<User>()
                           .Where(u => u.UserName == UserName)
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
    }
}
