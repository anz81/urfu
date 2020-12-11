namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddBasedOnLinkToWorkingPrograms : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DisciplineWorkingPrograms", "BasedOnId", c => c.Int());
            AddColumn("dbo.ModuleWorkingProgram", "BasedOnId", c => c.Int());
            CreateIndex("dbo.DisciplineWorkingPrograms", "BasedOnId");
            CreateIndex("dbo.ModuleWorkingProgram", "BasedOnId");
            AddForeignKey("dbo.DisciplineWorkingPrograms", "BasedOnId", "dbo.DisciplineWorkingPrograms", "VersionedDocumentId");
            AddForeignKey("dbo.ModuleWorkingProgram", "BasedOnId", "dbo.ModuleWorkingProgram", "VersionedDocumentId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ModuleWorkingProgram", "BasedOnId", "dbo.ModuleWorkingProgram");
            DropForeignKey("dbo.DisciplineWorkingPrograms", "BasedOnId", "dbo.DisciplineWorkingPrograms");
            DropIndex("dbo.ModuleWorkingProgram", new[] { "BasedOnId" });
            DropIndex("dbo.DisciplineWorkingPrograms", new[] { "BasedOnId" });
            DropColumn("dbo.ModuleWorkingProgram", "BasedOnId");
            DropColumn("dbo.DisciplineWorkingPrograms", "BasedOnId");
        }
    }
}
