namespace RazorPad.Web.EntityFramework.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class UserCascadeDelete : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("Credentials", "User_Id", "Users");
            DropIndex("Credentials", new[] { "User_Id" });
            AlterColumn("Credentials", "User_Id", c => c.Long(nullable: false));
            AddForeignKey("Credentials", "User_Id", "Users", "Id", cascadeDelete: true);
            CreateIndex("Credentials", "User_Id");
        }
        
        public override void Down()
        {
            DropIndex("Credentials", new[] { "User_Id" });
            DropForeignKey("Credentials", "User_Id", "Users");
            AlterColumn("Credentials", "User_Id", c => c.Long());
            CreateIndex("Credentials", "User_Id");
            AddForeignKey("Credentials", "User_Id", "Users", "Id");
        }
    }
}
