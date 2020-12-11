namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SubtroupMembershipIndexRebuild : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.SubgroupMemberships");
            AddPrimaryKey("dbo.SubgroupMemberships", new[] { "SubgroupId", "studentId" });
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.SubgroupMemberships");
            AddPrimaryKey("dbo.SubgroupMemberships", new[] { "studentId", "SubgroupId" });
        }
    }
}
