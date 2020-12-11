namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateIndexForApploads : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.Apploads", new[] { "grp", "status", "removed", "year", "eduDiscipline", "term" }, name: "IX_ApploadDto_auto1");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Apploads", "IX_ApploadDto_auto1");
        }
    }
}
