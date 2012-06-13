using System;
using System.Collections.Specialized;
using System.Security.Principal;
using System.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RazorPad.Web.EntityFramework;
using RazorPad.Web.Website.Controllers;
using RazorPad.Web.Website.Models;

namespace RazorPad.Web.Revisions
{
    [TestClass]
    public class SnippetsControllerTests
    {
        private RazorPadContext _razorPadContext;
        private Repository _repository;

        [TestInitialize]
        public void Init()
        {
            _razorPadContext = new RazorPadContext();
            _repository = new Repository(_razorPadContext, false);
        }


        [TestMethod]
        public void ShouldSaveNewAnonymousSnippet()
        {



        }

        [TestMethod]
        public void ShouldSaveNewOwnedSnippet()
        {
            var controller = new SnippetsController(_repository);
            controller.SetMockControllerContextWithUserAuthenticated("TestBot");
            var saveRequest = new SaveRequest
                                  {
                                      Template = @"@{ 
										            var Now = DateTime.Now.ToString(); 
									             }
									             <h1>Basic Integration Test</h1>
									             Test performed at: @Now
									            ",
                                      Title = "ShouldSaveNewOwnedSnippet Integration Test",
                                      Notes = "Having a meaningful note helps you in remembering things in future.",
                                  };
            var result = controller.SaveSnippet(saveRequest, false);
            Assert.AreEqual(result.Revision, 0);

        }

        [TestMethod]
        public void ShouldSaveExistingAnonymousSnippetWithRevision()
        {
        }

        [TestMethod]
        public void ShouldSaveExistingOwnedSnippetWithRevision()
        {
            var controller = new SnippetsController(_repository);
            controller.SetMockControllerContextWithUserAuthenticated("TestBot");
            var saveRequest = new SaveRequest
            {
                Template = @"@{ 
								var Now = DateTime.Now.ToString(); 
								}
								<h1>Basic Integration Test</h1>
								Test performed at: @Now
							",
                Title = "ShouldSaveExistingOwnedSnippetWithRevision Integration Test",
                Notes = "Having a meaningful note helps you in remembering things in future.",
            };
            var result = controller.SaveSnippet(saveRequest, false);
            Assert.AreEqual(result.Revision, 0);

            saveRequest.SnippetId = result.Key;

            result = controller.SaveSnippet(saveRequest, false);
            Assert.AreEqual(result.Revision, 1);


        }

        [TestMethod]
        public void ShouldCloneExistingOwnedSnippetAutomatically()
        {
            var controller = new SnippetsController(_repository);
            controller.SetMockControllerContextWithUserAuthenticated("TestBot");
            var saveRequest = new SaveRequest
            {
                Template = @"@{ 
								var Now = DateTime.Now.ToString(); 
								}
								<h1>Basic Integration Test</h1>
								Test performed at: @Now
							",
                Title = "ShouldCloneExistingSnippet Integration Test",
                Notes = "Having a meaningful note helps you in remembering things in future.",
            };
            var result = controller.SaveSnippet(saveRequest, false);
            Assert.AreEqual(result.Revision, 0);

            controller.SetMockControllerContextWithUserAuthenticated("TestBot2");
            var expectedCloneOf = saveRequest.SnippetId = result.Key;

            result = controller.SaveSnippet(saveRequest, false);
            Assert.AreEqual(result.Revision, 0);
            Assert.AreEqual(expectedCloneOf, result.CloneOf);
        }

        [TestMethod]
        public void ShouldCloneExistingSnippet()
        {
            var controller = new SnippetsController(_repository);
            controller.SetMockControllerContextWithUserAuthenticated("TestBot");
            var saveRequest = new SaveRequest
            {
                Template = @"@{ 
								var Now = DateTime.Now.ToString(); 
								}
								<h1>Basic Integration Test</h1>
								Test performed at: @Now
							",
                Title = "ShouldCloneExistingSnippet Integration Test",
                Notes = "Having a meaningful note helps you in remembering things in future.",

            };
            var result = controller.SaveSnippet(saveRequest, false);
            Assert.AreEqual(result.Revision, 0);
            
            var originalKey = saveRequest.SnippetId = result.Key;

            // clone explicitly
            result = controller.SaveSnippet(saveRequest, true);
            Assert.AreEqual(result.Revision, 0);
            Assert.AreNotEqual(originalKey, result.Key);
        }
    }
}
