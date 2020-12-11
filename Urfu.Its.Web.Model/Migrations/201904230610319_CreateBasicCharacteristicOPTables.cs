namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateBasicCharacteristicOPTables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BasicCharacteristicOP",
                c => new
                    {
                        VersionedDocumentId = c.Int(nullable: false),
                        EduProgramId = c.Int(nullable: false),
                        UpopStatusId = c.Int(),
                        StatusChangeTime = c.DateTime(nullable: false),
                        Version = c.Int(nullable: false),
                        StandardName = c.String(nullable: false, maxLength: 20),
                        BasedOnId = c.Int(),
                    })
                .PrimaryKey(t => t.VersionedDocumentId)
                .ForeignKey("dbo.BasicCharacteristicOP", t => t.BasedOnId)
                .ForeignKey("dbo.EduPrograms", t => t.EduProgramId, cascadeDelete: true)
                .ForeignKey("dbo.Standards", t => t.StandardName, cascadeDelete: true)
                .ForeignKey("dbo.UPOPStatuses", t => t.UpopStatusId)
                .ForeignKey("dbo.VersionedDocuments", t => t.VersionedDocumentId)
                .Index(t => t.VersionedDocumentId)
                .Index(t => t.EduProgramId)
                .Index(t => t.UpopStatusId)
                .Index(t => t.StandardName)
                .Index(t => t.BasedOnId);
            
            CreateTable(
                "dbo.BasicCharacteristicOPMapping",
                c => new
                    {
                        BasicCharacteristicOPId = c.Int(nullable: false),
                        ModuleWorkingProgramId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.BasicCharacteristicOPId, t.ModuleWorkingProgramId })
                .ForeignKey("dbo.BasicCharacteristicOP", t => t.BasicCharacteristicOPId, cascadeDelete: true)
                .ForeignKey("dbo.ModuleWorkingProgram", t => t.ModuleWorkingProgramId)
                .Index(t => t.BasicCharacteristicOPId)
                .Index(t => t.ModuleWorkingProgramId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.BasicCharacteristicOP", "VersionedDocumentId", "dbo.VersionedDocuments");
            DropForeignKey("dbo.BasicCharacteristicOP", "UpopStatusId", "dbo.UPOPStatuses");
            DropForeignKey("dbo.BasicCharacteristicOP", "StandardName", "dbo.Standards");
            DropForeignKey("dbo.BasicCharacteristicOPMapping", "ModuleWorkingProgramId", "dbo.ModuleWorkingProgram");
            DropForeignKey("dbo.BasicCharacteristicOPMapping", "BasicCharacteristicOPId", "dbo.BasicCharacteristicOP");
            DropForeignKey("dbo.BasicCharacteristicOP", "EduProgramId", "dbo.EduPrograms");
            DropForeignKey("dbo.BasicCharacteristicOP", "BasedOnId", "dbo.BasicCharacteristicOP");
            DropIndex("dbo.BasicCharacteristicOPMapping", new[] { "ModuleWorkingProgramId" });
            DropIndex("dbo.BasicCharacteristicOPMapping", new[] { "BasicCharacteristicOPId" });
            DropIndex("dbo.BasicCharacteristicOP", new[] { "BasedOnId" });
            DropIndex("dbo.BasicCharacteristicOP", new[] { "StandardName" });
            DropIndex("dbo.BasicCharacteristicOP", new[] { "UpopStatusId" });
            DropIndex("dbo.BasicCharacteristicOP", new[] { "EduProgramId" });
            DropIndex("dbo.BasicCharacteristicOP", new[] { "VersionedDocumentId" });
            DropTable("dbo.BasicCharacteristicOPMapping");
            DropTable("dbo.BasicCharacteristicOP");
        }
    }
}
