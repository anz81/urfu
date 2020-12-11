namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ForeignLanguage_RemoveMale : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.ForeignLanguagePeriods", "Male");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ForeignLanguagePeriods", "Male", c => c.Boolean());
        }
    }
}
