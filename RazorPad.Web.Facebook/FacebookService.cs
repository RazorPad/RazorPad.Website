using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;

namespace RazorPad.Web.Facebook
{
    public class FacebookService
    {
        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public IEnumerable<string> Permissions { get; set; }


        public FacebookService()
        {
            ClientId = ConfigurationManager.AppSettings["Facebook.AppId"];
            ClientSecret = ConfigurationManager.AppSettings["Facebook.AppSecret"];
            Permissions = new[] { "email" };
        }


        public AuthToken Authenticate(string code, string redirectUrl)
        {
            var urlBuilder = new StringBuilder("https://graph.facebook.com/oauth/access_token?");
            urlBuilder.AppendFormat("client_id={0}", ClientId);
            urlBuilder.AppendFormat("&client_secret={0}", ClientSecret);
            urlBuilder.AppendFormat("&redirect_uri={0}", redirectUrl);
            urlBuilder.AppendFormat("&code={0}", code);

            var url = urlBuilder.ToString();

            var authResponseContent = new WebClient().DownloadString(url);

            var parts = 
                authResponseContent.Split('&')
                    .Select(x => { 
                        var pair = x.Split('=');
                        return new KeyValuePair<string, string>(pair[0], pair[1]);
                    })
                    .ToDictionary(part => part.Key, part => part.Value);

            var expiration = int.Parse(parts["expires"]);

            return new AuthToken
                       {
                           Value = parts["access_token"],
                           Expiration = DateTime.Now.AddSeconds(expiration)
                       };
        }

        public string GetLoginUrl(string redirectUrl)
        {
            var urlBuilder = new StringBuilder("https://www.facebook.com/dialog/oauth?");
            urlBuilder.AppendFormat( "client_id={0}", ClientId);
            urlBuilder.AppendFormat("&redirect_uri={0}", redirectUrl);

            if (Permissions != null && Permissions.Any())
                urlBuilder.AppendFormat("&scope={0}", string.Join(",", Permissions));

            return urlBuilder.ToString();
        }

        public FacebookUser GetUser(AuthToken token)
        {
            var url = "https://graph.facebook.com/me?access_token=" + token.Value;
            
            var response = new WebClient().DownloadString(url);

            var user = new JavaScriptSerializer().Deserialize<FacebookUser>(response);

            return user;
        }
    }
}