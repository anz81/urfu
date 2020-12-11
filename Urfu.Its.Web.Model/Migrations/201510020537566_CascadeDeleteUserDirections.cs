namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CascadeDeleteUserDirections : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.UserDirections", "UserName", "dbo.AspNetUsers");
            AddForeignKey("dbo.UserDirections", "UserName", "dbo.AspNetUsers", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserDirections", "UserName", "dbo.AspNetUsers");
            AddForeignKey("dbo.UserDirections", "UserName", "dbo.AspNetUsers", "Id");
        }
    }
}
