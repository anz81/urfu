namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddWorkingProgramAuthorsAndDisciplineWorkingPrograms : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DisciplineWorkingPrograms",
                c => new
                    {
                        VersionedDocumentId = c.Int(nullable: false),
                        ModuleWorkingProgramId = c.Int(),
                        DisciplineId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.VersionedDocumentId)
                .ForeignKey("dbo.Disciplines", t => t.DisciplineId)
                .ForeignKey("dbo.ModuleWorkingProgram", t => t.ModuleWorkingProgramId)
                .ForeignKey("dbo.VersionedDocuments", t => t.VersionedDocumentId)
                .Index(t => t.VersionedDocumentId)
                .Index(t => t.ModuleWorkingProgramId)
                .Index(t => t.DisciplineId);
            
            CreateTable(
                "dbo.WorkingProgramAuthors",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LastName = c.String(),
                        FirstName = c.String(),
                        MiddleName = c.String(),
                        AcademicDegree = c.String(),
                        AcademicTitle = c.String(),
                        Post = c.String(),
                        Workplace = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Teachers", "academicTitle", c => c.String());
            AddColumn("dbo.Teachers", "academicDegree", c => c.String());
            DropColumn("dbo.VersionedDocumentBlocks", "SchemaSections");
        }
        
        public override void Down()
        {
            AddColumn("dbo.VersionedDocumentBlocks", "SchemaSections", c => c.String(nullable: false));
            DropForeignKey("dbo.DisciplineWorkingPrograms", "VersionedDocumentId", "dbo.VersionedDocuments");
            DropForeignKey("dbo.DisciplineWorkingPrograms", "ModuleWorkingProgramId", "dbo.ModuleWorkingProgram");
            DropForeignKey("dbo.DisciplineWorkingPrograms", "DisciplineId", "dbo.Disciplines");
            DropIndex("dbo.DisciplineWorkingPrograms", new[] { "DisciplineId" });
            DropIndex("dbo.DisciplineWorkingPrograms", new[] { "ModuleWorkingProgramId" });
            DropIndex("dbo.DisciplineWorkingPrograms", new[] { "VersionedDocumentId" });
            DropColumn("dbo.Teachers", "academicDegree");
            DropColumn("dbo.Teachers", "academicTitle");
            DropTable("dbo.WorkingProgramAuthors");
            DropTable("dbo.DisciplineWorkingPrograms");
        }
    }
}
