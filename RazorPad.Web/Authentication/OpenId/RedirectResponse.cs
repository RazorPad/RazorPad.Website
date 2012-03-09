using System.Web.Mvc;
using DotNetOpenAuth.Messaging;

namespace RazorPad.Web.Authentication.OpenId
{
    public class RedirectResponse : OpenIdAuthenticationResponse
    {
        public OutgoingWebResponse Response { get; private set; }

        public RedirectResponse(OutgoingWebResponse response)
        {
            Response = response;
        }

        public static implicit operator ActionResult(RedirectResponse response)
        {
            return response.Response.AsActionResult();
        }
    }
}