namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SectionFKCompetitionGroupValidation : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.SectionFKCompetitionGroups", "Name", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.SectionFKCompetitionGroups", "Name", c => c.String());
        }
    }
}
