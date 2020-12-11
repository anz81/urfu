namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SubgroupsExpectedChildcount : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Subgroups", "ExpectedChildCount", c => c.Double());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Subgroups", "ExpectedChildCount");
        }
    }
}
