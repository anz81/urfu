namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SectionFKCompetitionGroup_ShortName : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SectionFKCompetitionGroups", "ShortName", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SectionFKCompetitionGroups", "ShortName");
        }
    }
}
