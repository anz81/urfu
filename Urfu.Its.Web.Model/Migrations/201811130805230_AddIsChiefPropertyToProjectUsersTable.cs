namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIsChiefPropertyToProjectUsersTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProjectUsers", "IsChief", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProjectUsers", "IsChief");
        }
    }
}
