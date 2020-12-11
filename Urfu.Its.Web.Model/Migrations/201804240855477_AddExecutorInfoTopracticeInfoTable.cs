namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddExecutorInfoTopracticeInfoTable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.PracticeInfo", "PracticeTimeId", "dbo.PracticeTimes");
            DropForeignKey("dbo.PracticeInfo", "PracticeWayId", "dbo.PracticeWays");
            DropIndex("dbo.PracticeInfo", new[] { "PracticeWayId" });
            DropIndex("dbo.PracticeInfo", new[] { "PracticeTimeId" });
            AddColumn("dbo.PracticeInfo", "ExecutorName", c => c.String(maxLength: 255));
            AddColumn("dbo.PracticeInfo", "ExecutorPhone", c => c.String(maxLength: 255));
            AddColumn("dbo.PracticeInfo", "ExecutorEmail", c => c.String(maxLength: 255));
            AlterColumn("dbo.PracticeInfo", "PracticeWayId", c => c.Int());
            AlterColumn("dbo.PracticeInfo", "PracticeTimeId", c => c.Int());
            CreateIndex("dbo.PracticeInfo", "PracticeWayId");
            CreateIndex("dbo.PracticeInfo", "PracticeTimeId");
            AddForeignKey("dbo.PracticeInfo", "PracticeTimeId", "dbo.PracticeTimes", "Id");
            AddForeignKey("dbo.PracticeInfo", "PracticeWayId", "dbo.PracticeWays", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PracticeInfo", "PracticeWayId", "dbo.PracticeWays");
            DropForeignKey("dbo.PracticeInfo", "PracticeTimeId", "dbo.PracticeTimes");
            DropIndex("dbo.PracticeInfo", new[] { "PracticeTimeId" });
            DropIndex("dbo.PracticeInfo", new[] { "PracticeWayId" });
            AlterColumn("dbo.PracticeInfo", "PracticeTimeId", c => c.Int(nullable: false));
            AlterColumn("dbo.PracticeInfo", "PracticeWayId", c => c.Int(nullable: false));
            DropColumn("dbo.PracticeInfo", "ExecutorEmail");
            DropColumn("dbo.PracticeInfo", "ExecutorPhone");
            DropColumn("dbo.PracticeInfo", "ExecutorName");
            CreateIndex("dbo.PracticeInfo", "PracticeTimeId");
            CreateIndex("dbo.PracticeInfo", "PracticeWayId");
            AddForeignKey("dbo.PracticeInfo", "PracticeWayId", "dbo.PracticeWays", "Id", cascadeDelete: true);
            AddForeignKey("dbo.PracticeInfo", "PracticeTimeId", "dbo.PracticeTimes", "Id", cascadeDelete: true);
        }
    }
}
