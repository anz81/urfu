namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameForeingContentToProfilesTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Profiles", "FOREIGN_CONTENT", c => c.String());
            DropColumn("dbo.Profiles", "ForeingContent");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Profiles", "ForeingContent", c => c.String());
            DropColumn("dbo.Profiles", "FOREIGN_CONTENT");
        }
    }
}
