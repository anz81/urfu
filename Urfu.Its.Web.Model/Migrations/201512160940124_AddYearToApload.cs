namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddYearToApload : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Apploads", "year", c => c.Int(nullable: false,defaultValue:2015));
            AddColumn("dbo.Apploads", "term", c => c.Int(nullable: false, defaultValue: 1));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Apploads", "term");
            DropColumn("dbo.Apploads", "year");
        }
    }
}
