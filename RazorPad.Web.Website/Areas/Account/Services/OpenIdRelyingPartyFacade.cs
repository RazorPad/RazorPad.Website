using System;
using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OpenId;
using DotNetOpenAuth.OpenId.Extensions.SimpleRegistration;
using DotNetOpenAuth.OpenId.RelyingParty;

namespace RazorPad.Web.Website.Areas.Account.Services
{
    public class OpenIdRelyingPartyFacade
    {
        public Func<ClaimsRequest> ClaimsFactory { get; set; }


        public OpenIdRelyingPartyFacade()
        {
            ClaimsFactory = () => null;
        }


        public OpenIdAuthenticationResponse Authenticate(string provider)
        {
            Identifier id;

            try
            {
                id = Identifier.Parse(provider);
            }
            catch (ArgumentException ex)
            {
                return new InvalidIdentifier(provider, ex);
            }

            return Authenticate(id);
        }

        public OpenIdAuthenticationResponse Authenticate(Identifier provider)
        {
            var openid = new OpenIdRelyingParty();
            var response = openid.GetResponse();

            if (response == null)
            {
                try
                {
                    var authenticationRequest = openid.CreateRequest(provider);

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
                return new SuccessfulResponse(response);
            }

            return new FailedResponse(response);
        }
    }

    public abstract class OpenIdAuthenticationResponse
    {
    }

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

    public class RedirectResponse : OpenIdAuthenticationResponse
    {
        public OutgoingWebResponse Response { get; private set; }

        public RedirectResponse(OutgoingWebResponse response)
        {
            Response = response;
        }
    }

    public class FailedResponse : OpenIdAuthenticationResponse
    {
        public IAuthenticationResponse Response { get; private set; }

        public FailedResponse(IAuthenticationResponse response)
        {
            Response = response;
        }
    }

    public class AuthenticationException : OpenIdAuthenticationResponse
    {
        public Exception Exception { get; private set; }

        public AuthenticationException(Exception exception)
        {
            Exception = exception;
        }
    }

    public class InvalidIdentifier : AuthenticationException
    {
        public string Identifier { get; private set; }

        public InvalidIdentifier(string identifier, Exception exception)
            : base(exception)
        {
            Identifier = identifier;
        }
    }
}