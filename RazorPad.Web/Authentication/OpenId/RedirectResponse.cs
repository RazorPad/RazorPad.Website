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
            return new OutgoingWebResponseActionResult(response.Response);
        }


        internal class OutgoingWebResponseActionResult : ActionResult
        {
            private readonly OutgoingWebResponse response;

            internal OutgoingWebResponseActionResult(OutgoingWebResponse response)
            {
                this.response = response;
            }

            public override void ExecuteResult(ControllerContext context)
            {
                response.Send();
            }
        }
    }
}