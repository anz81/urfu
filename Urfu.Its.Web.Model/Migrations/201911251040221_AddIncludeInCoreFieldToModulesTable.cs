namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIncludeInCoreFieldToModulesTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Modules", "IncludeInCore", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Modules", "IncludeInCore");
        }
    }
}
