using DotNetOpenAuth.OpenId.Extensions.SimpleRegistration;
using DotNetOpenAuth.OpenId.RelyingParty;

namespace RazorPad.Web.Authentication.OpenId
{
    public class SuccessfulResponse : OpenIdAuthenticationResponse
    {
        public string ClaimedIdentifier
        {
            get { return Response.ClaimedIdentifier; }
        }

        public ClaimsResponse Credentials
        {
            get { return Response.GetExtension<ClaimsResponse>(); }
        }

        public IAuthenticationResponse Response { get; private set; }

        public SuccessfulResponse(IAuthenticationResponse response)
        {
            Response = response;
        }
    }
}