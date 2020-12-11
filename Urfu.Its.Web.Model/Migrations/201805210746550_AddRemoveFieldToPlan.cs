namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddRemoveFieldToPlan : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Plans", "remove", c => c.Boolean(nullable: false, defaultValue: true));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Plans", "remove");
        }
    }
}
