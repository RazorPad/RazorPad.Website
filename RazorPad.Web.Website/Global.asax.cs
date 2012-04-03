using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace RazorPad.Web.Website
{
	public class Global : System.Web.HttpApplication
	{
		// Need this for error controller
		protected void Application_Error(object sender, EventArgs e)
		{
			Exception ex = Server.GetLastError();

			Application[HttpContext.Current.Request.UserHostAddress.ToString()] = ex;
		}
	}
}