namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddRoleToProjectStudentSelectionPriorityTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProjectStudentSelectionPriorities", "roleId", c => c.Int());
            CreateIndex("dbo.ProjectStudentSelectionPriorities", "roleId");
            AddForeignKey("dbo.ProjectStudentSelectionPriorities", "roleId", "dbo.ProjectRoles", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProjectStudentSelectionPriorities", "roleId", "dbo.ProjectRoles");
            DropIndex("dbo.ProjectStudentSelectionPriorities", new[] { "roleId" });
            DropColumn("dbo.ProjectStudentSelectionPriorities", "roleId");
        }
    }
}
