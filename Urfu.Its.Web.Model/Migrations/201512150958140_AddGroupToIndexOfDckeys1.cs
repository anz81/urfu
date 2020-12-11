namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddGroupToIndexOfDckeys1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Apploads", "discipline", c => c.String(maxLength: 256));
            CreateIndex("dbo.Apploads", "discipline", name: "IX_ApploadDto_discipline");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Apploads", "IX_ApploadDto_discipline");
            AlterColumn("dbo.Apploads", "discipline", c => c.String());
        }
    }
}
