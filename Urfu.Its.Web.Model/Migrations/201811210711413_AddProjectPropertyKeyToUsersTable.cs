namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddProjectPropertyKeyToUsersTable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Projects", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Projects", new[] { "ApplicationUser_Id" });
            DropColumn("dbo.Projects", "ApplicationUser_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Projects", "ApplicationUser_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.Projects", "ApplicationUser_Id");
            AddForeignKey("dbo.Projects", "ApplicationUser_Id", "dbo.AspNetUsers", "Id");
        }
    }
}
