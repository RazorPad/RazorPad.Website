using System.Web.Mvc;

[assembly: WebActivator.PostApplicationStartMethod(typeof(RazorPad.Web.Website.App_Start.GlobalFilters), "Start")]

namespace RazorPad.Web.Website.App_Start
{
    public class GlobalFilters
    {
        public static void Start()
        {
            var filters = System.Web.Mvc.GlobalFilters.Filters;

            //filters.Add(new HandleErrorAttribute());
        }
    }
}