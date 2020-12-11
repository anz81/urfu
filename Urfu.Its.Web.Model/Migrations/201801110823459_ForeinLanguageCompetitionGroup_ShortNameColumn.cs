namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ForeinLanguageCompetitionGroup_ShortNameColumn : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ForeignLanguageCompetitionGroups", "ShortName", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ForeignLanguageCompetitionGroups", "ShortName");
        }
    }
}
