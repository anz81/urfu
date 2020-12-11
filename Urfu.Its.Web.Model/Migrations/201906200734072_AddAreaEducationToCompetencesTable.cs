namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAreaEducationToCompetencesTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Competences", "AreaEducationId", c => c.Int());
            CreateIndex("dbo.Competences", "AreaEducationId");
            AddForeignKey("dbo.Competences", "AreaEducationId", "dbo.AreaEducation", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Competences", "AreaEducationId", "dbo.AreaEducation");
            DropIndex("dbo.Competences", new[] { "AreaEducationId" });
            DropColumn("dbo.Competences", "AreaEducationId");
        }
    }
}
