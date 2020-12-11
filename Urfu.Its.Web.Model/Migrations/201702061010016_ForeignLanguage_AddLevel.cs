namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ForeignLanguage_AddLevel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ForeignLanguageSubgroups", "Level", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ForeignLanguageSubgroups", "Level");
        }
    }
}
