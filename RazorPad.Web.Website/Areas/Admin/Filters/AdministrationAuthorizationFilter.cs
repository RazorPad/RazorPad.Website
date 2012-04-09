using System.Web.Mvc;

namespace RazorPad.Web.Website.Areas.Admin.Filters
{
    public class AdministrationAuthorizationFilter : AuthorizeAttribute
    {
        public AdministrationAuthorizationFilter()
        {
            Roles = "Admin";
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if(AdminAreaRegistration.Name == (string)filterContext.RouteData.DataTokens["area"])
                base.OnAuthorization(filterContext);
        }
    }
}