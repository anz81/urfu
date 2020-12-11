namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DWP_Remove_Upopstatus : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.DisciplineWorkingPrograms", "UpopStatusId", "dbo.UPOPStatuses");
            DropIndex("dbo.DisciplineWorkingPrograms", new[] { "UpopStatusId" });
            DropColumn("dbo.DisciplineWorkingPrograms", "StatusChangeTime");
            DropColumn("dbo.DisciplineWorkingPrograms", "UpopStatusId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.DisciplineWorkingPrograms", "UpopStatusId", c => c.Int());
            AddColumn("dbo.DisciplineWorkingPrograms", "StatusChangeTime", c => c.DateTime(nullable: false));
            CreateIndex("dbo.DisciplineWorkingPrograms", "UpopStatusId");
            AddForeignKey("dbo.DisciplineWorkingPrograms", "UpopStatusId", "dbo.UPOPStatuses", "Id");
        }
    }
}
