using System;
using System.Web;
using System.Web.Mvc;

namespace RazorPad.Website.Extensions
{
    public static class UrlExtensions
    {

        public static string ExternalUrl(this UrlHelper url, string relativeUrl)
        {
            return ExternalUrl(url.RequestContext.HttpContext.Request, relativeUrl);
        }

        public static string ExternalUrl(this HtmlHelper html, string relativeUrl)
        {
            return ExternalUrl(html.ViewContext.HttpContext.Request, relativeUrl);
        }

        public static string ExternalUrl(this HttpRequestBase request, string relativeUrl)
        {
            return ExternalUrl(request.Url, relativeUrl);
        }

        public static string ExternalUrl(Uri requestUrl, string relativeUrl)
        {
            var absoluteUrl = relativeUrl;

            if(relativeUrl.StartsWith("~/"))
                absoluteUrl = VirtualPathUtility.ToAbsolute(relativeUrl);

            var externalUrl = string.Format("{0}://{1}{2}", requestUrl.Scheme, requestUrl.Authority, absoluteUrl);

            return externalUrl;
        }

    }
}