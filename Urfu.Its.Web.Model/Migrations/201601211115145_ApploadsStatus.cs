namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ApploadsStatus : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Apploads", "removed", c => c.Boolean(nullable: false));
            AddColumn("dbo.Apploads", "status", c => c.Int(nullable: false));
            AddColumn("dbo.MetaSubgroups", "Selectable", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.MetaSubgroups", "Selectable");
            DropColumn("dbo.Apploads", "status");
            DropColumn("dbo.Apploads", "removed");
        }
    }
}
