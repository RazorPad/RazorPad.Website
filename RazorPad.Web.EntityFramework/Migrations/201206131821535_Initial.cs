namespace RazorPad.Web.EntityFramework.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "Users",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        DateCreated = c.DateTime(nullable: false),
                        EmailAddress = c.String(nullable: false),
                        LastSeen = c.DateTime(),
                        Username = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "Credentials",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Hash = c.String(),
                        ForgotPasswordToken = c.String(),
                        Expiration = c.DateTime(),
                        Token = c.String(),
                        UserId = c.String(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                        User_Id = c.Long(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("Users", t => t.User_Id)
                .Index(t => t.User_Id);
            
            CreateTable(
                "Snippets",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        CreatedBy = c.String(nullable: false),
                        Revision = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        Key = c.String(nullable: false),
                        Model = c.String(),
                        Notes = c.String(maxLength: 1000),
                        Title = c.String(maxLength: 500),
                        View = c.String(nullable: false),
                        CloneOf = c.String(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropIndex("Credentials", new[] { "User_Id" });
            DropForeignKey("Credentials", "User_Id", "Users");
            DropTable("Snippets");
            DropTable("Credentials");
            DropTable("Users");
        }
    }
}
