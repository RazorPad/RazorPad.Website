using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Moq;

namespace RazorPad.Web
{
    /// <summary>
    /// Helper class for mocking Http context in controllers
    /// </summary>
    public static class MockMvcHelpers
    {
        /// <summary>
        /// Mocks the HTTP context.
        /// </summary>
        /// <returns></returns>
        public static HttpContextBase MockHttpContext()
        {
            var context = new Mock<HttpContextBase>();
            var request = new Mock<HttpRequestBase>();
            var response = new Mock<HttpResponseBase>();
            var session = new Mock<HttpSessionStateBase>();
            var server = new Mock<HttpServerUtilityBase>();
            var form = new NameValueCollection();

            request.Setup(r => r.Form).Returns(form);
            context.Setup(ctx => ctx.Request).Returns(request.Object);
            context.Setup(ctx => ctx.Response).Returns(response.Object);
            context.Setup(ctx => ctx.Session).Returns(session.Object);
            context.Setup(ctx => ctx.Server).Returns(server.Object);

            return context.Object;
        }

        /// <summary>
        /// Mocks the HTTP context.
        /// </summary>
        /// <param name="principal">The principal that will be associated with the Face Http Context</param>
        /// <returns></returns>
        public static HttpContextBase MockHttpContext(IPrincipal principal)
        {
            var context = new Mock<HttpContextBase>();
            var request = new Mock<HttpRequestBase>();
            var response = new Mock<HttpResponseBase>();
            var session = new Mock<HttpSessionStateBase>();
            var server = new Mock<HttpServerUtilityBase>();
            var form = new NameValueCollection();
            var user = principal;

            request.Setup(r => r.Form).Returns(form);
            context.Setup(ctx => ctx.Request).Returns(request.Object);
            context.Setup(ctx => ctx.Response).Returns(response.Object);
            context.Setup(ctx => ctx.Session).Returns(session.Object);
            context.Setup(ctx => ctx.Server).Returns(server.Object);
            context.Setup(ctx => ctx.User).Returns(user);


            return context.Object;
        }

        /// <summary>
        /// Sets the Mock controller context with the specified user authenticated.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="valueCollection">Collection of values that will be added to the context</param>
        public static void SetMockControllerContextWithUserAuthenticated(this Controller controller, string userName, NameValueCollection valueCollection)
        {
            var identity = new MockIdentity(userName);
            var httpContext = MockHttpContext(new MockPrincipal(identity, null));

            httpContext.SkipAuthorization = true;
            if (httpContext.Request.IsAuthenticated)
                httpContext.User = new GenericPrincipal(new GenericIdentity(userName), new [] { "User" });

            if (valueCollection != null)
                httpContext.Request.Form.Add(valueCollection);

            var context = new ControllerContext(new RequestContext(httpContext, new RouteData()), controller);
            controller.ControllerContext = context;
        }

        /// <summary>
        /// Sets the Mock controller context with the specified user authenticated.
        /// </summary>
        /// <param name="controller">The controller.</param>
        /// <param name="userName">Name of the user.</param>
        public static void SetMockControllerContextWithUserAuthenticated(this Controller controller, string userName)
        {
            SetMockControllerContextWithUserAuthenticated(controller, userName, null);
        }

        /// <summary>
        /// Sets the Mock controller's context with anonymous user.
        /// </summary>
        /// <param name="controller">The controller.</param>
        public static void SetMockControllerContextWithUserAuthenticated(this Controller controller)
        {
            SetMockControllerContextWithUserAuthenticated(controller, null, null);
        }
 


    }
}
