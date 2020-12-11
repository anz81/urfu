namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDepartment : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EduPrograms", "departmentId", c => c.String(maxLength: 127));
            CreateIndex("dbo.EduPrograms", "departmentId");
            AddForeignKey("dbo.EduPrograms", "departmentId", "dbo.Divisions", "uuid");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.EduPrograms", "departmentId", "dbo.Divisions");
            DropIndex("dbo.EduPrograms", new[] { "departmentId" });
            DropColumn("dbo.EduPrograms", "departmentId");
        }
    }
}
