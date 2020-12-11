namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCompetenceGroupFieldToCompetencesTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Competences", "CompetenceGroupId", c => c.Int());
            CreateIndex("dbo.Competences", "CompetenceGroupId");
            AddForeignKey("dbo.Competences", "CompetenceGroupId", "dbo.CompetenceGroups", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Competences", "CompetenceGroupId", "dbo.CompetenceGroups");
            DropIndex("dbo.Competences", new[] { "CompetenceGroupId" });
            DropColumn("dbo.Competences", "CompetenceGroupId");
        }
    }
}
