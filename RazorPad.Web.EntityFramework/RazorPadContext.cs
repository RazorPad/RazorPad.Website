using System;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Utilities;
using System.Data.Objects;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Transactions;

namespace RazorPad.Web.EntityFramework
{
	public class RazorPadContext : DbContext
	{
		public DbSet<User> Users { get; set; }
		public DbSet<Snippet> Snippets { get; set; }

		public RazorPadContext()
			: base("RazorPad")
		{
			Database.SetInitializer(new RazorPadContextInitializer());
		}

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{/*
            modelBuilder.Entity<User>()
                .HasRequired(x => x.Credentials)
                .WithRequiredPrincipal()
                .WillCascadeOnDelete(true);
           */
		}

		public class RazorPadContextInitializer : IDatabaseInitializer<RazorPadContext>
		{
			static Lazy<bool> IsAppHarbor = new Lazy<bool>(() =>
				!string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["appharbor.commit_id"]));

			public void InitializeDatabase(RazorPadContext context)
			{
                if (!IsAppHarbor.Value)
                {
                    bool flag;
                    using (new TransactionScope(TransactionScopeOption.Suppress))
                        flag = context.Database.Exists();
                    if (flag)
                    {
                        if (context.Database.CompatibleWithModel(true))
                            return;
                        context.Database.Delete();
                    }
                    context.Database.Create();
                    context.Database.ExecuteSqlCommand(RazorPadContextScripts.CreateInsertSnippetProc);
                    this.Seed(context);
                    context.SaveChanges();
                    
                }
			}

            /// <summary>
            /// A that should be overridden to actually add data to the context for seeding.
            ///                 The default implementation does nothing.
            /// 
            /// </summary>
            /// <param name="context">The context to seed.</param>
            protected virtual void Seed(RazorPadContext context)
            {
            }
		}

        

		public void InsertSnippet(Snippet snippet)
		{
			var revision = InsertSnippet(snippet.View, snippet.Model, snippet.Title, snippet.Notes, snippet.Key,
								 snippet.CreatedBy, snippet.CloneOf, snippet.GetType().Name);
		    snippet.Revision = revision;
		}

		/// <summary>
		/// Inserts a new snippet for a given user. If the snippet exists, adds a new revision.
		/// Takes care of cloning if the user is not the owner of the snippet.
		/// </summary>
		/// <param name="view">No Metadata Documentation available.</param>
		/// <param name="model">No Metadata Documentation available.</param>
		/// <param name="title">No Metadata Documentation available.</param>
		/// <param name="notes">No Metadata Documentation available.</param>
		/// <param name="key">No Metadata Documentation available.</param>
		/// <param name="isRevision">No Metadata Documentation available.</param>
		/// <param name="createdBy">No Metadata Documentation available.</param>
		/// <param name="cloneOf">No Metadata Documentation available.</param>
		private int InsertSnippet(string view, string model, string title, string notes, string key, string createdBy, string cloneOf, string discriminator)
		{
            if(string.IsNullOrWhiteSpace(view))
                throw new ArgumentOutOfRangeException("view", "View cannot be null or empty");

            if (string.IsNullOrWhiteSpace(createdBy))
                throw new ArgumentOutOfRangeException("createdBy", "CreatedBy cannot be null or empty");

            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentOutOfRangeException("key", "Key cannot be null or empty");

            if (string.IsNullOrWhiteSpace(discriminator))
                throw new ArgumentOutOfRangeException("discriminator", "Discriminator cannot be null or empty");
            

            // Essential Params
			var viewParameter = new SqlParameter("View", view);
            var keyParameter = new SqlParameter("Key", key);
            var createdByParameter = new SqlParameter("CreatedBy", createdBy);
            var discriminatorParameter = new SqlParameter("Discriminator", discriminator);

            // Nullable params
            var modelParameter = new SqlParameter("@Model", model ?? SqlString.Null);
			var titleParameter = new SqlParameter("Title", title ??SqlString.Null);
            var notesParameter = new SqlParameter("Notes", notes ?? SqlString.Null);
            var cloneOfParameter = new SqlParameter("CloneOf", cloneOf ?? SqlString.Null);


			return Database.SqlQuery<int>("EXEC [dbo].[InsertSnippet] @View, @Model, @Title, @Notes, @Key, @CreatedBy, @CloneOf, @Discriminator",
				viewParameter, modelParameter, titleParameter, notesParameter, keyParameter,
                createdByParameter, cloneOfParameter, discriminatorParameter).SingleOrDefault();
		}


	}

    public static class RazorPadContextScripts
    {
        public const string CreateInsertSnippetProc = @"CREATE PROCEDURE [dbo].[InsertSnippet](
	                                                        @View 				NVARCHAR(MAX),
	                                                        @Model 				NVARCHAR(MAX) = NULL,
	                                                        @Title 				NVARCHAR(500) = NULL, 
	                                                        @Notes 				NVARCHAR(1000) = NULL, 	
	                                                        @Key 				NVARCHAR(MAX),
	                                                        @CreatedBy 			NVARCHAR(MAX),
	                                                        @CloneOf			NVARCHAR(MAX) = NULL,
	                                                        @Discriminator		NVARCHAR(128)
                                                        )
                                                        AS
                                                        BEGIN

	                                                        INSERT INTO SNIPPETS
		                                                        ([CloneOf]
		                                                        ,[CreatedBy]
		                                                        ,[DateCreated]
		                                                        ,[Key]
		                                                        ,[Model]
		                                                        ,[Notes]
		                                                        ,[Title]
		                                                        ,[View]
		                                                        ,[Revision]
		                                                        ,[Discriminator])
	                                                        OUTPUT Inserted.Revision
	                                                        SELECT 
		                                                        @cloneOf,
		                                                        @CreatedBy, 
		                                                        CURRENT_TIMESTAMP,
		                                                        @Key, 
		                                                        @Model, 
		                                                        @Notes, 
		                                                        @Title, 
		                                                        @View, 
		                                                        ISNULL ((SELECT TOP 1 Revision + 1
				                                                           FROM [dbo].[Snippets]
				                                                          WHERE [Key] = @Key 				
			                                                           ORDER BY Revision DESC), 0),
		                                                        @Discriminator
                                                        END
                                                        ";
    }
}





