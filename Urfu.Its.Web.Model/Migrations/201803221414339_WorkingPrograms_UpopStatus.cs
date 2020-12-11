namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class WorkingPrograms_UpopStatus : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.DisciplineWorkingPrograms", "StatusChangeTime", c => c.DateTime(nullable: false));
            AddColumn("dbo.DisciplineWorkingPrograms", "UpopStatusId", c => c.Int());
            AddColumn("dbo.ModuleWorkingProgram", "UpopStatusId", c => c.Int());
            CreateIndex("dbo.DisciplineWorkingPrograms", "UpopStatusId");
            CreateIndex("dbo.ModuleWorkingProgram", "UpopStatusId");
            AddForeignKey("dbo.ModuleWorkingProgram", "UpopStatusId", "dbo.UPOPStatuses", "Id");
            AddForeignKey("dbo.DisciplineWorkingPrograms", "UpopStatusId", "dbo.UPOPStatuses", "Id");
            DropColumn("dbo.ModuleWorkingProgram", "Status");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ModuleWorkingProgram", "Status", c => c.Int(nullable: false));
            DropForeignKey("dbo.DisciplineWorkingPrograms", "UpopStatusId", "dbo.UPOPStatuses");
            DropForeignKey("dbo.ModuleWorkingProgram", "UpopStatusId", "dbo.UPOPStatuses");
            DropIndex("dbo.ModuleWorkingProgram", new[] { "UpopStatusId" });
            DropIndex("dbo.DisciplineWorkingPrograms", new[] { "UpopStatusId" });
            DropColumn("dbo.ModuleWorkingProgram", "UpopStatusId");
            DropColumn("dbo.DisciplineWorkingPrograms", "UpopStatusId");
            DropColumn("dbo.DisciplineWorkingPrograms", "StatusChangeTime");
        }
    }
}
