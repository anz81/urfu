namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddRemovedFieldToMUPTables : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MUPs", "Removed", c => c.Boolean(nullable: false));
            AddColumn("dbo.MUPSubgroups", "Removed", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.MUPSubgroups", "Removed");
            DropColumn("dbo.MUPs", "Removed");
        }
    }
}
