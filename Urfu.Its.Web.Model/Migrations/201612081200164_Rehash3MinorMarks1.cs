namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Rehash3MinorMarks1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MinorSubgroupMemberships", "Mark", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.MinorSubgroupMemberships", "Mark");
        }
    }
}
