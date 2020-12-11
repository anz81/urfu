namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddForeingContentToProfilesTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Profiles", "ForeingContent", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Profiles", "ForeingContent");
        }
    }
}
