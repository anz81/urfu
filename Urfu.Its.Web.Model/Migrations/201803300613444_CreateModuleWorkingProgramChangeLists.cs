namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateModuleWorkingProgramChangeLists : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ModuleWorkingProgramChangeLists",
                c => new
                    {
                        VersionedDocumentId = c.Int(nullable: false),
                        SourceId = c.Int(nullable: false),
                        TargetId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.VersionedDocumentId)
                .ForeignKey("dbo.VersionedDocuments", t => t.VersionedDocumentId)
                .ForeignKey("dbo.ModuleWorkingProgram", t => t.SourceId)
                .ForeignKey("dbo.ModuleWorkingProgram", t => t.TargetId)
                .Index(t => t.VersionedDocumentId)
                .Index(t => t.SourceId)
                .Index(t => t.TargetId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ModuleWorkingProgramChangeLists", "TargetId", "dbo.ModuleWorkingProgram");
            DropForeignKey("dbo.ModuleWorkingProgramChangeLists", "SourceId", "dbo.ModuleWorkingProgram");
            DropForeignKey("dbo.ModuleWorkingProgramChangeLists", "VersionedDocumentId", "dbo.VersionedDocuments");
            DropIndex("dbo.ModuleWorkingProgramChangeLists", new[] { "TargetId" });
            DropIndex("dbo.ModuleWorkingProgramChangeLists", new[] { "SourceId" });
            DropIndex("dbo.ModuleWorkingProgramChangeLists", new[] { "VersionedDocumentId" });
            DropTable("dbo.ModuleWorkingProgramChangeLists");
        }
    }
}
