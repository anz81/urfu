namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MoveVersionToVersionedDocumentBlock : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.VersionedDocumentBlocks", "DocumentTemplateId", "dbo.VersionedDocumentTemplates");
            DropIndex("dbo.VersionedDocumentBlocks", new[] { "DocumentTemplateId" });
            AddColumn("dbo.VersionedDocumentBlockLinks", "UpdateTime", c => c.DateTime(nullable: false));
            AddColumn("dbo.VersionedDocumentBlocks", "Version", c => c.Int(nullable: false));
            DropColumn("dbo.VersionedDocumentBlockLinks", "CreatedAt");
            DropColumn("dbo.VersionedDocumentBlockLinks", "Version");
            DropColumn("dbo.VersionedDocumentBlocks", "DocumentTemplateId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.VersionedDocumentBlocks", "DocumentTemplateId", c => c.Int(nullable: false));
            AddColumn("dbo.VersionedDocumentBlockLinks", "Version", c => c.Int(nullable: false));
            AddColumn("dbo.VersionedDocumentBlockLinks", "CreatedAt", c => c.DateTime(nullable: false));
            DropColumn("dbo.VersionedDocumentBlocks", "Version");
            DropColumn("dbo.VersionedDocumentBlockLinks", "UpdateTime");
            CreateIndex("dbo.VersionedDocumentBlocks", "DocumentTemplateId");
            AddForeignKey("dbo.VersionedDocumentBlocks", "DocumentTemplateId", "dbo.VersionedDocumentTemplates", "Id");
        }
    }
}
