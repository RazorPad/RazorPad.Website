using System;
using System.Web;
using System.Web.Mvc;

namespace RazorPad.Web
{
    public static class UrlExtensions
    {
        public static string ExternalAction(this UrlHelper url, string actionName, string controllerName, object routeValues = null)
        {
            var requestUrl = url.Action(actionName, controllerName, routeValues);
            return ExternalUrl(url, requestUrl);
        }

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

            var hostPath = new UriBuilder
            {
                Host = requestUrl.Host,
                Path = "/",
                Port = requestUrl.Port,
                Scheme = requestUrl.Scheme,
            };

#if(AppHarbor)
            // AppHarbor External Url fix
            hostPath.Port = 80;
#endif

            return new Uri(hostPath.Uri, absoluteUrl).AbsoluteUri;
        }
    }
}