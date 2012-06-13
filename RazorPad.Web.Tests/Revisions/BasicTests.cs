using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RazorPad.Web.EntityFramework;

namespace RazorPad.Web.Revisions
{
	[TestClass]
	public class BasicTests
	{
		private RazorPadContext context;
		string view, model, title, notes, key, createdBy, cloneOf;
		private bool? isRevision;
		private Snippet snippet;

		[TestInitialize]
		public void Init()
		{
			context = new RazorPadContext();

		}

		private void SetupTestDefaults()
		{
			snippet = new Snippet()
			          	{
			          		View = @"@{ 
										var Now = DateTime.Now.ToString(); 
									 }
									 <h1>Basic Integration Test</h1>
									 Test performed at: @Now
									",
			          		Title = "Basic Integration Test",
			          		Notes = "Having a meaningful note helps you in remembering things in future.",
			          		CreatedBy = "Anonymous",
			          		Revision = 0,
			          	};
		}

		[TestMethod]
		public void ShouldSaveAnonymousSnippetWithoutRevision()
		{
			SetupTestDefaults();
			context.InsertSnippet(snippet);
            Assert.AreEqual(snippet.Revision, 0);
		}

		[TestMethod]
		public void ShouldGetAnonymousSnippetWithoutRevision()
		{

		}

		[TestMethod]
		public void ShouldSaveAnonymousSnippetWithRevision()
		{
			SetupTestDefaults();

			// create first insert
			context.InsertSnippet(snippet);
            Assert.AreEqual(snippet.Revision, 0);

			// alter the view
			snippet.View += "<p>I&quot;m a revision!</p>";

			// save again to get a revision
			context.InsertSnippet(snippet);
            Assert.IsTrue(snippet.Revision > 0);
		}

		[TestMethod]
		public void ShouldGetAnonymousSnippetWithRevision()
		{

		}


		[TestMethod]
		public void ShouldSaveOwnedSnippetWithoutRevision()
		{
			SetupTestDefaults();
			createdBy = "TestBot";
			context.InsertSnippet(snippet);
            Assert.AreEqual(snippet.Revision, 0);
		}

		[TestMethod]
		public void ShouldGetOwnedSnippetWithoutRevision()
		{

		}

		[TestMethod]
		public void ShouldSaveOwnedSnippetWithRevision()
		{
			SetupTestDefaults();
			createdBy = "TestBot";

			// create first insert
			context.InsertSnippet(snippet);
            Assert.AreEqual(snippet.Revision, 0);

			// alter the view
			snippet.View += "<p>I&quot;m a revision!</p>";

			// save again to get a revision
            context.InsertSnippet(snippet);
            Assert.IsTrue(snippet.Revision > 0);
		}

		[TestMethod]
		public void ShouldGetOwnedSnippetWithRevision()
		{

		}
	}
}
