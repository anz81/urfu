namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddStandardAndVersionToWorkingPrograms : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DisciplineWorkingPrograms", "Version", c => c.Int(nullable: false));
            AddColumn("dbo.DisciplineWorkingPrograms", "StandardName", c => c.String(nullable: false, maxLength: 20));
            AddColumn("dbo.ModuleWorkingProgram", "Version", c => c.Int(nullable: false));
            AddColumn("dbo.ModuleWorkingProgram", "StandardName", c => c.String(nullable: false, maxLength: 20));
            CreateIndex("dbo.DisciplineWorkingPrograms", "StandardName");
            CreateIndex("dbo.ModuleWorkingProgram", "StandardName");
            AddForeignKey("dbo.ModuleWorkingProgram", "StandardName", "dbo.Standards", "Name", cascadeDelete: true);
            AddForeignKey("dbo.DisciplineWorkingPrograms", "StandardName", "dbo.Standards", "Name", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DisciplineWorkingPrograms", "StandardName", "dbo.Standards");
            DropForeignKey("dbo.ModuleWorkingProgram", "StandardName", "dbo.Standards");
            DropIndex("dbo.ModuleWorkingProgram", new[] { "StandardName" });
            DropIndex("dbo.DisciplineWorkingPrograms", new[] { "StandardName" });
            DropColumn("dbo.ModuleWorkingProgram", "StandardName");
            DropColumn("dbo.ModuleWorkingProgram", "Version");
            DropColumn("dbo.DisciplineWorkingPrograms", "StandardName");
            DropColumn("dbo.DisciplineWorkingPrograms", "Version");
        }
    }
}
