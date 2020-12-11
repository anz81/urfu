namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddRemovedFieldToSubgroupsTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Subgroups", "Removed", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Subgroups", "Removed");
        }
    }
}
