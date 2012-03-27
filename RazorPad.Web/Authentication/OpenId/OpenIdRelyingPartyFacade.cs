using System;
using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OpenId;
using DotNetOpenAuth.OpenId.Extensions.AttributeExchange;
using DotNetOpenAuth.OpenId.Extensions.SimpleRegistration;
using DotNetOpenAuth.OpenId.RelyingParty;

namespace RazorPad.Web.Authentication.OpenId
{
    public class OpenIdRelyingPartyFacade
    {
        public const string GoogleProviderUrl = "https://www.google.com/accounts/o8/id";

        public OpenIdAuthenticationResponse Google()
        {
            return Authenticate(GoogleProviderUrl);
        }

        public OpenIdAuthenticationResponse Authenticate(string provider)
        {
            var openid = new OpenIdRelyingParty();
            var response = openid.GetResponse();

            if (response == null)
            {
                try
                {
                    Identifier providerId;

                    try
                    {
                        providerId = Identifier.Parse(provider);
                    }
                    catch (ArgumentException ex)
                    {
                        return new InvalidIdentifier(provider, ex);
                    }

                    var authenticationRequest = openid.CreateRequest(providerId);

                    authenticationRequest.AddExtension(new ClaimsRequest {Email = DemandLevel.Require});

                    var fetch = new FetchRequest();
                    fetch.Attributes.Add(new AttributeRequest(WellKnownAttributes.Contact.Email, true));
                    authenticationRequest.AddExtension(fetch);

                    return new RedirectResponse(authenticationRequest.RedirectingResponse);
                }
                catch (ProtocolException ex)
                {
                    return new AuthenticationException(ex);
                }
            }

            if (response.Status == AuthenticationStatus.Authenticated)
            {
                return new SuccessfulResponse(response);
            }

            return new FailedResponse(response);
        }
    }
}