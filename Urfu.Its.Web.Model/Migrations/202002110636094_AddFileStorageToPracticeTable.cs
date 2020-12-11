namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFileStorageToPracticeTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PracticeDocuments", "FileStorageId", c => c.Int());
            AddColumn("dbo.PracticeDecrees", "FileStorageId", c => c.Int());
            CreateIndex("dbo.PracticeDocuments", "FileStorageId");
            CreateIndex("dbo.PracticeDecrees", "FileStorageId");
            AddForeignKey("dbo.PracticeDocuments", "FileStorageId", "dbo.FileStorage", "Id");
            AddForeignKey("dbo.PracticeDecrees", "FileStorageId", "dbo.FileStorage", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PracticeDecrees", "FileStorageId", "dbo.FileStorage");
            DropForeignKey("dbo.PracticeDocuments", "FileStorageId", "dbo.FileStorage");
            DropIndex("dbo.PracticeDecrees", new[] { "FileStorageId" });
            DropIndex("dbo.PracticeDocuments", new[] { "FileStorageId" });
            DropColumn("dbo.PracticeDecrees", "FileStorageId");
            DropColumn("dbo.PracticeDocuments", "FileStorageId");
        }
    }
}
