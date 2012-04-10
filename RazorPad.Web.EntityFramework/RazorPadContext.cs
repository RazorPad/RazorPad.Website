using System;
using System.Configuration;
using System.Data.Entity;

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
        {
            modelBuilder.Entity<User>()
                .HasRequired(x => x.Credentials)
                .WithRequiredPrincipal()
                .WillCascadeOnDelete(true);
        }

        public class RazorPadContextInitializer : IDatabaseInitializer<RazorPadContext>
        {
            static Lazy<bool> IsAppHarbor = new Lazy<bool>(() =>
                !string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["appharbor.commit_id"]));

            public void InitializeDatabase(RazorPadContext context)
            {
                if (!IsAppHarbor.Value)
                    new DropCreateDatabaseIfModelChanges<RazorPadContext>().InitializeDatabase(context);
            }
        }
    }
}
