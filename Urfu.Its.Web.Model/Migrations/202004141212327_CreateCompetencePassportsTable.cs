namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateCompetencePassportsTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CompetencePassports",
                c => new
                    {
                        VersionedDocumentId = c.Int(nullable: false),
                        BasicCharacteristicOPId = c.Int(nullable: false),
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
            DropForeignKey("dbo.CompetencePassports", "VersionedDocumentId", "dbo.VersionedDocuments");
            DropForeignKey("dbo.CompetencePassports", "UpopStatusId", "dbo.UPOPStatuses");
            DropForeignKey("dbo.CompetencePassports", "FileStorageDocxId", "dbo.FileStorage");
            DropForeignKey("dbo.CompetencePassports", "BasicCharacteristicOPId", "dbo.BasicCharacteristicOP");
            DropIndex("dbo.CompetencePassports", new[] { "FileStorageDocxId" });
            DropIndex("dbo.CompetencePassports", new[] { "UpopStatusId" });
            DropIndex("dbo.CompetencePassports", new[] { "BasicCharacteristicOPId" });
            DropIndex("dbo.CompetencePassports", new[] { "VersionedDocumentId" });
            DropTable("dbo.CompetencePassports");
        }
    }
}
