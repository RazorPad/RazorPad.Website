using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RazorPad.Web.Website.Controllers
{
	public class ErrorController : Controller
	{
		public ActionResult HttpError()
		{
			Exception ex = null;

			try
			{
				ex = (Exception)HttpContext.Application[Request.UserHostAddress.ToString()];
			}
			catch { }

			if (ex != null && !string.IsNullOrEmpty(ex.StackTrace) && ex.StackTrace.ToLower().Contains("newrelic"))
			{
				// Temp workaround for the startup YSOD
				return RedirectToAction("Index", "RazorPad");
			}

			ViewBag.Title = "Oops. We're sorry. An error occurred and we're on the case.";

			return View("Error");
		}

		public ActionResult Http404()
		{
			ViewBag.Title = "The page you requested was not found";
			return View("Error");
		}

		// Redirect to home when /Error is navigated to directly
		public ActionResult Index()
		{
			return RedirectToAction("Index", "RazorPad");
		}
		
	}


}
