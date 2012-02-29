using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;

namespace RazorPad.Web.Facebook
{
    public class FacebookUser
    {
        public string EmailAddress { get; set; }
        public uint UserId { get; set; }
    }

    public class FacebookService
    {
        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public IEnumerable<string> Permissions { get; set; }


        public FacebookService()
        {
            ClientId = ConfigurationManager.AppSettings["Facebook.ClientID"];
            ClientSecret = ConfigurationManager.AppSettings["Facebook.ClientSecret"];
            Permissions = new[] { "email" };
        }


        public FacebookUser Authenticate(string code, string redirectUrl)
        {
            var urlBuilder = new StringBuilder("https://graph.facebook.com/oauth/access_token?");
            urlBuilder.AppendFormat("client_id={0}", ClientId);
            urlBuilder.AppendFormat("&client_secret={0}", ClientSecret);
            urlBuilder.AppendFormat("&code={0}", code);
            urlBuilder.AppendFormat("&redirect_uri={0}", redirectUrl);

            var authResponseContent = new WebClient().DownloadString(urlBuilder.ToString());
            var facebookUser = new JavaScriptSerializer().Deserialize<FacebookUser>(authResponseContent);
            return facebookUser;
        }

        public string GetLoginUrl(string redirectUrl)
        {
            var urlBuilder = new StringBuilder("https://www.facebook.com/dialog/oauth?");
            urlBuilder.AppendFormat( "client_id={0}", ClientId);
            urlBuilder.AppendFormat("&redirect_uri={0}", HttpUtility.UrlEncode(redirectUrl));

            if (Permissions != null && Permissions.Any())
                urlBuilder.AppendFormat("&scope={0}", string.Join(",", Permissions));

            return urlBuilder.ToString();
        }
    }
}