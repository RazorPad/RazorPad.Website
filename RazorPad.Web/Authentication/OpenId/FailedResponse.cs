using DotNetOpenAuth.OpenId.RelyingParty;

namespace RazorPad.Web.Authentication.OpenId
{
    public class FailedResponse : OpenIdAuthenticationResponse
    {
        public IAuthenticationResponse Response { get; private set; }

        public FailedResponse(IAuthenticationResponse response)
        {
            Response = response;
        }
    }
}