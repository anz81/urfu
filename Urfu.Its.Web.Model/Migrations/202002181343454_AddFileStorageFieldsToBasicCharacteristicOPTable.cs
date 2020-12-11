namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFileStorageFieldsToBasicCharacteristicOPTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BasicCharacteristicOP", "FileStorageDocxId", c => c.Int());
            AddColumn("dbo.BasicCharacteristicOP", "FileStorageZipId", c => c.Int());
            CreateIndex("dbo.BasicCharacteristicOP", "FileStorageDocxId");
            CreateIndex("dbo.BasicCharacteristicOP", "FileStorageZipId");
            AddForeignKey("dbo.BasicCharacteristicOP", "FileStorageDocxId", "dbo.FileStorage", "Id");
            AddForeignKey("dbo.BasicCharacteristicOP", "FileStorageZipId", "dbo.FileStorage", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.BasicCharacteristicOP", "FileStorageZipId", "dbo.FileStorage");
            DropForeignKey("dbo.BasicCharacteristicOP", "FileStorageDocxId", "dbo.FileStorage");
            DropIndex("dbo.BasicCharacteristicOP", new[] { "FileStorageZipId" });
            DropIndex("dbo.BasicCharacteristicOP", new[] { "FileStorageDocxId" });
            DropColumn("dbo.BasicCharacteristicOP", "FileStorageZipId");
            DropColumn("dbo.BasicCharacteristicOP", "FileStorageDocxId");
        }
    }
}
