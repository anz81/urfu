namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateRemovedInMUPModeusTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MUPModeus", "Removed", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.MUPModeus", "Removed");
        }
    }
}
