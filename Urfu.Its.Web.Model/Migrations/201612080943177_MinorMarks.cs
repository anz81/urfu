namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MinorMarks : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MinorSubgroupMemberships", "Rating", c => c.Int());
            AddColumn("dbo.MinorSubgroupMemberships", "Mark", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.MinorSubgroupMemberships", "Mark");
            DropColumn("dbo.MinorSubgroupMemberships", "Rating");
        }
    }
}
