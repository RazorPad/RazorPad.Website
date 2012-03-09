using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RazorPad.Web.Facebook
{
    [TestClass]
    [Ignore]
    public class FacebookServiceTests
    {
        private FacebookService _service;

        [TestInitialize]
        public void TestInitialize()
        {
            _service = new FacebookService { LocalEndpoint = "http://razorpad.apphb.com/" };
        }

        [TestMethod]
        public void ShouldProduceLoginUrl()
        {
            var loginUrl = _service.GetLoginUrl();

            Assert.IsNotNull(loginUrl);
        }

        [TestMethod]
        public void ShouldAuthenticateUser()
        {
            var credential = _service.Authenticate("REPLACE_WITH_YOUR_TOKEN");

            Assert.IsNotNull(credential.Token);
        }

        [TestMethod]
        public void ShouldGetUser()
        {
            var token = _service.Authenticate("REPLACE_WITH_YOUR_TOKEN");

            var user = _service.GetUser(token);

            Assert.IsNotNull(user.Id);
        }
    }
}
