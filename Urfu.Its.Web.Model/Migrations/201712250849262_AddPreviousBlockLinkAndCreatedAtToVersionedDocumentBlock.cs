namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPreviousBlockLinkAndCreatedAtToVersionedDocumentBlock : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.VersionedDocumentBlocks", "CreatedAt", c => c.DateTime(nullable: false));
            AddColumn("dbo.VersionedDocumentBlocks", "PreviousBlockId", c => c.Int());
            CreateIndex("dbo.VersionedDocumentBlocks", "PreviousBlockId");
            AddForeignKey("dbo.VersionedDocumentBlocks", "PreviousBlockId", "dbo.VersionedDocumentBlocks", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.VersionedDocumentBlocks", "PreviousBlockId", "dbo.VersionedDocumentBlocks");
            DropIndex("dbo.VersionedDocumentBlocks", new[] { "PreviousBlockId" });
            DropColumn("dbo.VersionedDocumentBlocks", "PreviousBlockId");
            DropColumn("dbo.VersionedDocumentBlocks", "CreatedAt");
        }
    }
}
