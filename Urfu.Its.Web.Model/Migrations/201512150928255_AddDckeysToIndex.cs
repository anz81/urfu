namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDckeysToIndex : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Apploads", "IX_ApploadDto_eduDiscipline");
            AlterColumn("dbo.Apploads", "dckey", c => c.String(maxLength: 256));
            CreateIndex("dbo.Apploads", new[] { "eduDiscipline", "dckey" }, name: "IX_ApploadDto_eduDiscipline");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Apploads", "IX_ApploadDto_eduDiscipline");
            AlterColumn("dbo.Apploads", "dckey", c => c.String());
            CreateIndex("dbo.Apploads", "eduDiscipline", name: "IX_ApploadDto_eduDiscipline");
        }
    }
}
