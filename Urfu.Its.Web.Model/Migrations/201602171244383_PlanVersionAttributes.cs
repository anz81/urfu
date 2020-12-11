namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PlanVersionAttributes : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Plans", "versionTitle", c => c.String());
            Sql("update plans set versionTitle = versionNumber");
            Sql("update plans set versionNumber = '1'");
            AddColumn("dbo.Plans", "versionActive", c => c.Boolean(nullable: false));
            AlterColumn("dbo.Plans", "versionNumber", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Plans", "versionNumber", c => c.String());
            DropColumn("dbo.Plans", "versionActive");
            DropColumn("dbo.Plans", "versionTitle");
        }
    }
}
