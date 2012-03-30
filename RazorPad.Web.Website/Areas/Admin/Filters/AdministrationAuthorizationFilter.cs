using System.Web.Mvc;

namespace RazorPad.Web.Website.Areas.Admin.Filters
{
    public class AdministrationAuthorizationFilter : AuthorizeAttribute
    {
        public AdministrationAuthorizationFilter()
        {
            Roles = "Administrator";
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if(AdminAreaRegistration.Name == (string)filterContext.RouteData.Values["area"])
                base.OnAuthorization(filterContext);
        }
    }
}