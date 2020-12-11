namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NullsInApploadPracticeFlows : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Apploads", "practiceFlows", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Apploads", "practiceFlows", c => c.Int(nullable: false));
        }
    }
}
