using System.Web.Mvc;
using System.Web.Routing;

[assembly: WebActivator.PostApplicationStartMethod(typeof(RazorPad.Web.Website.App_Start.Routing), "Start")]

namespace RazorPad.Web.Website.App_Start
{
    public class Routing
    {
        public static void Start()
        {
            AreaRegistration.RegisterAllAreas();

            var routes = RouteTable.Routes;

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // Named route to home page
            routes.MapRoute(
                "Home",
                "",
                new { controller = "RazorPad", action = "Index", area = string.Empty }
            );

            // Named route to login page
            routes.MapRoute(
                "Login",
                "login",
                new { controller = "FormsAuth", action = "Login", area = "Account" }
            );

            // Named route to logout page
            routes.MapRoute(
                "Logout",
                "logout",
                new { controller = "FormsAuth", action = "Logout", area = "Account" }
            );

            routes.MapRoute(
                "Snippet",
                "{id}",
                new { controller = "RazorPad", action = "Index", id = UrlParameter.Optional, area = string.Empty }
            );

            routes.MapRoute(
                "User",
                "users/{username}",
                new { controller = "Users", action = "Index", area = string.Empty },
                new[] { "RazorPad.Web.Website.Controllers" }
            );
            routes.MapRoute(
                "UserSnippets",
                "users/{username}/snippets",
                new { controller = "Users", action = "Snippets", area = string.Empty },
                new[] { "RazorPad.Web.Website.Controllers" }
            );


            routes.MapRoute(
                "Default",
                "{controller}/{action}/{id}",
                new { controller = "RazorPad", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}