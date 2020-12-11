namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RehashMinorMarks1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.MinorSubgroupMemberships", "Mark", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.MinorSubgroupMemberships", "Mark", c => c.Int(nullable: false));
        }
    }
}
