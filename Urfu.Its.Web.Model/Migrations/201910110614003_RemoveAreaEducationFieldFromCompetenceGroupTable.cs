namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveAreaEducationFieldFromCompetenceGroupTable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.CompetenceGroups", "AreaEducationId", "dbo.AreaEducation");
            DropIndex("dbo.CompetenceGroups", new[] { "AreaEducationId" });
            DropColumn("dbo.CompetenceGroups", "AreaEducationId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.CompetenceGroups", "AreaEducationId", c => c.Int());
            CreateIndex("dbo.CompetenceGroups", "AreaEducationId");
            AddForeignKey("dbo.CompetenceGroups", "AreaEducationId", "dbo.AreaEducation", "Id");
        }
    }
}
