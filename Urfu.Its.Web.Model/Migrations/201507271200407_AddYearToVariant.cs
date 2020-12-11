namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddYearToVariant : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Variants", "Year", c => c.Int(nullable: false,defaultValue: 2015));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Variants", "Year");
        }
    }
}
