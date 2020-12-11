namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ForeignLanguage_Description : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ForeignLanguageSubgroups", "Description", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ForeignLanguageSubgroups", "Description");
        }
    }
}
