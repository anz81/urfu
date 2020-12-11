namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddProfileToProjectCompetencesTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProjectCompetences", "ProfileId", c => c.String(maxLength: 127));
            CreateIndex("dbo.ProjectCompetences", "ProfileId");
            AddForeignKey("dbo.ProjectCompetences", "ProfileId", "dbo.Profiles", "ID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProjectCompetences", "ProfileId", "dbo.Profiles");
            DropIndex("dbo.ProjectCompetences", new[] { "ProfileId" });
            DropColumn("dbo.ProjectCompetences", "ProfileId");
        }
    }
}
