using System;
using System.Configuration;
using System.Web;

namespace RazorPad.Web.Website
{
	public class Global : HttpApplication
	{
	    public static string Version
	    {
            get
            {
                var appHarborCommit = ConfigurationManager.AppSettings["appharbor.commit_id"];

                if (string.IsNullOrWhiteSpace(appHarborCommit))
                    return typeof (Global).Assembly.GetName().Version.ToString();
                
                return appHarborCommit;
            }
	    }

		// Need this for error controller
		protected void Application_Error(object sender, EventArgs e)
		{
			Exception ex = Server.GetLastError();

			Application[HttpContext.Current.Request.UserHostAddress] = ex;
		}
	}
}