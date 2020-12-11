namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RehashMinorMarks : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MinorSubgroupMemberships", "Score", c => c.Int());
            DropColumn("dbo.MinorSubgroupMemberships", "Rating");
        }
        
        public override void Down()
        {
            AddColumn("dbo.MinorSubgroupMemberships", "Rating", c => c.Int());
            DropColumn("dbo.MinorSubgroupMemberships", "Score");
        }
    }
}
