namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSubdivisionToPracticeInfo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PracticeInfo", "Subdivision", c => c.String(maxLength: 255));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PracticeInfo", "Subdivision");
        }
    }
}
