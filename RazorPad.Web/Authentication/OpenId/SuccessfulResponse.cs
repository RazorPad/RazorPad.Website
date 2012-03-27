using DotNetOpenAuth.OpenId.Extensions.AttributeExchange;
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

        public string EmailAddress
        {
            get
            {
                string email = null;
                
                if(Credentials != null)
                    email = Credentials.Email;

                if(string.IsNullOrWhiteSpace(email))
                {
                    var fetch = Response.GetExtension<FetchResponse>();
                    if (fetch != null)
                        email = fetch.GetAttributeValue(WellKnownAttributes.Contact.Email);
                }
                
                return email;
            }
        }

        public string FriendlyIdentifier
        {
            get
            {
                string id = Response.FriendlyIdentifierForDisplay;

                // Google returns the same identifier for 
                // everyone -- ignore it
                if (id == null || id.StartsWith("www.google"))
                    return ClaimedIdentifier;

                return id;
            }
        }

        public bool HasEmailAddress
        {
            get { return !string.IsNullOrWhiteSpace(EmailAddress); }
        }

        public IAuthenticationResponse Response { get; private set; }

        public SuccessfulResponse(IAuthenticationResponse response)
        {
            Response = response;
        }
    }
}