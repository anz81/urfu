namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddNameToVersionedDocumentBlock : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.VersionedDocumentBlocks", "Name", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.VersionedDocumentBlocks", "Name");
        }
    }
}
