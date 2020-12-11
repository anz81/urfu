namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addRemoveFieldToProfileTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Profiles", "remove", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Profiles", "remove");
        }
    }
}
