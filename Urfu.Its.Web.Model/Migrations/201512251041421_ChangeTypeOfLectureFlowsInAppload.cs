namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeTypeOfLectureFlowsInAppload : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Apploads", "lectureFlows", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Apploads", "lectureFlows", c => c.String());
        }
    }
}
