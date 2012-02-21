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
        public ActionResult Login(string UserName, string Password)
        {
            if (!string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(Password))
            {
                if (ValidateUser(UserName, Password))
                    RedirectFromLoginPage(UserName, Password);
                else
                    ViewBag["errorMsg"] = "Login failed! Make sure you have entered the right user name and password!";
            }

            return View("Login");
        }

        public ActionResult Register(User userInfo)
        {
            if (userInfo != null)
            {
                var session = DataDocumentStore.Instance.OpenSession();
                userInfo.DateCreated = DateTime.UtcNow;
                session.Store(userInfo);
                session.SaveChanges();
                session.Dispose();

                RedirectToAction("Login", "Account");
            }
            return View("Register");
        }

        private bool ValidateUser(string UserName, string Password)
        {
            bool isValid = true;

            return isValid;
        }
        private void RedirectFromLoginPage(string UserName, string ReturnUrl)
        {
            FormsAuthentication.SetAuthCookie(UserName, false);

            if (!string.IsNullOrEmpty(ReturnUrl))
                Response.Redirect(ReturnUrl);
            else
                Response.Redirect(FormsAuthentication.DefaultUrl);
        }
    }
}
