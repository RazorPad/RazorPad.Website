using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RazorPad.Web.Facebook
{
    [TestClass]
    public class FacebookServiceTests
    {
        private const string ApplicationUrl = "http://razorpad.apphb.com/";

        private FacebookService _service;

        [TestInitialize]
        public void TestInitialize()
        {
            _service = new FacebookService()
                           {
                               ClientId = "[SECRET]",
                               ClientSecret = "[SECRET]"
                           };
        }

        [TestMethod]
        public void ShouldProduceLoginUrl()
        {
            var loginUrl = _service.GetLoginUrl(ApplicationUrl);

            Assert.IsNotNull(loginUrl);
        }

        [TestMethod]
        public void ShouldAuthenticateUser()
        {
            var token = _service
                .Authenticate("REPLACE_WITH_YOUR_TOKEN", 
                ApplicationUrl);

            Assert.IsNotNull(token.Value);
        }

        [TestMethod]
        public void ShouldGetUser()
        {
            var token = _service
                .Authenticate("REPLACE_WITH_YOUR_TOKEN", 
                ApplicationUrl);

            var user = _service.GetUser(token);

            Assert.IsNotNull(user.UserId);
        }
    }
}
