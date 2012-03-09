using System.Web;
using System.Web.Mvc;

namespace RazorPad.Web.Authentication.Facebook
{
    public static class FacebookExtensions
    {
        public static IHtmlString FacebookLogin(this UrlHelper url)
        {
            var loginUrl = new FacebookService().GetLoginUrl();

            return new HtmlString(loginUrl);
        }

        public static IHtmlString FacebookLogin(this HtmlHelper html, string text = null)
        {
            var facebookUrl = new UrlHelper(html.ViewContext.RequestContext).FacebookLogin().ToString();

            var linkTag = new TagBuilder("a");
            linkTag.AddCssClass("facebook-login");
            linkTag.MergeAttribute("href", facebookUrl);
            linkTag.SetInnerText(text ?? "Facebook Login");

            return new HtmlString(linkTag.ToString());
        }

    }
}