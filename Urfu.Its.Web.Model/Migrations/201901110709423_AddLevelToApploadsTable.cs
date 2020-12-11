namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddLevelToApploadsTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Apploads", "Level", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Apploads", "Level");
        }
    }
}
