namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAnnotationFieldToModulesTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Modules", "annotation", c => c.String());
            CreateIndex("dbo.CompetencePassports", "BasedOnId");
            AddForeignKey("dbo.CompetencePassports", "BasedOnId", "dbo.CompetencePassports", "VersionedDocumentId");
        }
        
        public override void Down()
        {
            DropColumn("dbo.Modules", "annotation");
            DropForeignKey("dbo.CompetencePassports", "BasedOnId", "dbo.CompetencePassports");
            DropIndex("dbo.CompetencePassports", new[] { "BasedOnId" });
        }
    }
}
