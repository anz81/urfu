namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateModuleAnnotationTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ModuleAnnotations",
                c => new
                    {
                        VersionedDocumentId = c.Int(nullable: false),
                        BasicCharacteristicOPId = c.Int(nullable: false),
                        PlanNumber = c.Int(nullable: false),
                        PlanVersionNumber = c.Int(nullable: false),
                        UpopStatusId = c.Int(),
                        StatusChangeTime = c.DateTime(nullable: false),
                        Comment = c.String(),
                        FileStorageDocxId = c.Int(),
                    })
                .PrimaryKey(t => t.VersionedDocumentId)
                .ForeignKey("dbo.BasicCharacteristicOP", t => t.BasicCharacteristicOPId, cascadeDelete: true)
                .ForeignKey("dbo.FileStorage", t => t.FileStorageDocxId)
                .ForeignKey("dbo.UPOPStatuses", t => t.UpopStatusId)
                .ForeignKey("dbo.VersionedDocuments", t => t.VersionedDocumentId)
                .Index(t => t.VersionedDocumentId)
                .Index(t => t.BasicCharacteristicOPId)
                .Index(t => t.UpopStatusId)
                .Index(t => t.FileStorageDocxId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ModuleAnnotations", "VersionedDocumentId", "dbo.VersionedDocuments");
            DropForeignKey("dbo.ModuleAnnotations", "UpopStatusId", "dbo.UPOPStatuses");
            DropForeignKey("dbo.ModuleAnnotations", "FileStorageDocxId", "dbo.FileStorage");
            DropForeignKey("dbo.ModuleAnnotations", "BasicCharacteristicOPId", "dbo.BasicCharacteristicOP");
            DropIndex("dbo.ModuleAnnotations", new[] { "FileStorageDocxId" });
            DropIndex("dbo.ModuleAnnotations", new[] { "UpopStatusId" });
            DropIndex("dbo.ModuleAnnotations", new[] { "BasicCharacteristicOPId" });
            DropIndex("dbo.ModuleAnnotations", new[] { "VersionedDocumentId" });
            DropTable("dbo.ModuleAnnotations");
        }
    }
}
