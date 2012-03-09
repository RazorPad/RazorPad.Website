using System.Web.Mvc;
using RazorPad.Web.Authentication;
using RazorPad.Web.Authentication.OpenId;
using RazorPad.Web.Services;
using RazorPad.Web.Website.Extensions;

namespace RazorPad.Web.Website.Areas.Account.Controllers
{
    public class OpenIdController : Controller
    {
        private readonly OpenIdRelyingPartyFacade _openId;
        private readonly IRepository _repository;

        public OpenIdController(OpenIdRelyingPartyFacade openId, IRepository repository)
        {
            _openId = openId;
            _repository = repository;
        }

        public ActionResult Google(string returnUrl = null)
        {
            var response = _openId.Google();
            return ProcessOpenIdResponse(response);
        }

        [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Get), ValidateInput(false)]
        public ActionResult Authenticate(string provider, string returnUrl = null)
        {
            var response = _openId.Authenticate(provider);
            return ProcessOpenIdResponse(response);
        }

        protected virtual ActionResult ProcessOpenIdResponse(OpenIdAuthenticationResponse response, string returnUrl = null)
        {
            var redirect = response as RedirectResponse;
            if (redirect != null)
                return redirect;

            var success = response as SuccessfulResponse;
            if(success != null)
            {
                var user = _repository.FindUserByCredential<IntegratedAuthenticationCredential>(
                    credential => credential.Token == success.ClaimedIdentifier);

                if (user == null)
                    return RegisterNewUser(success);
                
                FormsAuthController.AuthenticateUser(user);

                return Redirect(Url.ExternalUrl(returnUrl ?? "~/"));
            }

            var exception = response as AuthenticationException;
            if (exception != null)
            {
                if (exception is InvalidIdentifier)
                {
                    TempData.Add("Error", "Invalid OpenID identifier");
                    TempData.Add("OpenId", ((InvalidIdentifier)exception).Identifier);
                }
                else
                {
                    TempData.Add("Error", "Error validating OpenID: " + exception.Exception.Message);
                }
            }

            return Redirect(Url.Login(returnUrl));
        }

        private ActionResult RegisterNewUser(SuccessfulResponse success)
        {
            // TODO: Create info cookie and Redirect to New User Registration
            FormsAuthController.AuthenticateUser(success.ClaimedIdentifier);
            return Redirect("~/");
        }
    }
}
