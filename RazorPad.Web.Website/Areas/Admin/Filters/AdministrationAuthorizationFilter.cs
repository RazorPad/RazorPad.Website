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
            if(filterContext.RouteData.Values["area"] == AdminAreaRegistration.Name)
                base.OnAuthorization(filterContext);
        }
    }
}