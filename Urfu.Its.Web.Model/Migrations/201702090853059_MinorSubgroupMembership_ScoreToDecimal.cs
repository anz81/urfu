namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MinorSubgroupMembership_ScoreToDecimal : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.MinorSubgroupMemberships", "Score", c => c.Decimal(precision: 4, scale: 1));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.MinorSubgroupMemberships", "Score", c => c.Int());
        }
    }
}
