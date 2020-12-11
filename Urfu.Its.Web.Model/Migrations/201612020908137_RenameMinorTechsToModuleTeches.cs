namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameMinorTechsToModuleTeches : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.MinorTeches", newName: "ModuleTeches");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.ModuleTeches", newName: "MinorTeches");
        }
    }
}
