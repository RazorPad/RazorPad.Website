using System.Web.Mvc;
using System.Web.Routing;
using RazorPad.Website.Models;

namespace RazorPad.Website
{
    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");


            routes.MapRoute(
                "mock", // Route name
                "mock", // URL with parameters
                new { controller = "Mock", action = "Index" } // Parameter defaults
            );

            routes.MapRoute(
                "Default", // Route name
                "{Controller}/{action}/{id}", // URL with parameters
                new { controller = "RazorPad", action = "Index", id = UrlParameter.Optional } // Parameter defaults
                );

        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);


            DataDocumentStore.Initialize();
        }

    }
}