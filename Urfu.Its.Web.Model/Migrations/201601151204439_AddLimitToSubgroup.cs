namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddLimitToSubgroup : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Subgroups", "Limit", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Subgroups", "Limit");
        }
    }
}
