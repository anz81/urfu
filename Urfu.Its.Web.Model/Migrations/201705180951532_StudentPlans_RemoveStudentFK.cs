namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class StudentPlans_RemoveStudentFK : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.StudentPlans", "StudentId", "dbo.Students");
            DropIndex("dbo.StudentPlans", new[] { "StudentId" });
        }
        
        public override void Down()
        {
            CreateIndex("dbo.StudentPlans", "StudentId");
            AddForeignKey("dbo.StudentPlans", "StudentId", "dbo.Students", "Id", cascadeDelete: true);
        }
    }
}
