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
        }
    }
}
