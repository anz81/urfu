namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Rehash2MinorMarks1 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.MinorSubgroupMemberships", "Mark");
        }
        
        public override void Down()
        {
            AddColumn("dbo.MinorSubgroupMemberships", "Mark", c => c.Int());
        }
    }
}
