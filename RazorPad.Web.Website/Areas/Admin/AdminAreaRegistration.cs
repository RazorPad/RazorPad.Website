using System.Web.Mvc;
using RazorPad.Web.Website.Areas.Admin.Filters;

namespace RazorPad.Web.Website.Areas.Admin
{
    public class AdminAreaRegistration : AreaRegistration
    {
        public const string Name = "Admin";

        public override string AreaName
        {
            get { return Name; }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Admin_default",
                "Admin/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );

            GlobalFilters.Filters.Add(new AdministrationAuthorizationFilter());
        }
    }
}
