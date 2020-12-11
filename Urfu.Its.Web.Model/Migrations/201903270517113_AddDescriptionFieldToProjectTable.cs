namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDescriptionFieldToProjectTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Projects", "Description", c => c.String());
            AddColumn("dbo.Projects", "Target", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Projects", "Target");
            DropColumn("dbo.Projects", "Description");
        }
    }
}
