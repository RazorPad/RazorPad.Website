using System;
using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OpenId;
using DotNetOpenAuth.OpenId.Extensions.SimpleRegistration;
using DotNetOpenAuth.OpenId.RelyingParty;

namespace RazorPad.Web.Authentication.OpenId
{
    public class OpenIdRelyingPartyFacade
    {
        public Func<ClaimsRequest> ClaimsFactory { get; set; }


        public OpenIdRelyingPartyFacade()
        {
            ClaimsFactory = () => new ClaimsRequest { Email = DemandLevel.Require, };
        }


        public OpenIdAuthenticationResponse Google()
        {
            return Authenticate("https://www.google.com/accounts/o8/id");
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

                    var claims = ClaimsFactory();

                    if (claims != null)
                        authenticationRequest.AddExtension(claims);

                    return new RedirectResponse(authenticationRequest.RedirectingResponse);
                }
                catch (ProtocolException ex)
                {
                    return new AuthenticationException(ex);
                }
            }

            if (response.Status == AuthenticationStatus.Authenticated)
            {
                var claims = response.GetExtension<ClaimsResponse>();
                return new SuccessfulResponse(response);
            }

            return new FailedResponse(response);
        }
    }
}