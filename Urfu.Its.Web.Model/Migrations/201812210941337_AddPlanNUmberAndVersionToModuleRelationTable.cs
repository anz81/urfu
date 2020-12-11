namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPlanNUmberAndVersionToModuleRelationTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ModuleRelations", "eduplanNumber", c => c.Int(nullable: false));
            AddColumn("dbo.ModuleRelations", "versionNumber", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ModuleRelations", "versionNumber");
            DropColumn("dbo.ModuleRelations", "eduplanNumber");
        }
    }
}
