﻿using System.Web.Mvc;
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

            // Named route to login page
            routes.MapRoute(
                "Login",
                "login",
                new { controller = "Account", action = "Login" }
            );

            routes.MapRoute(
                "Default",
                "{Controller}/{action}/{id}",
                new { controller = "RazorPad", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}