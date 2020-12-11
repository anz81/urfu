namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddProjectPropertyToProjectUsersTable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ProjectApplicationUsers", "Project_ModuleId", "dbo.Projects");
            DropForeignKey("dbo.ProjectApplicationUsers", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.ProjectUsers", "ProjectId", "dbo.Projects");
            DropIndex("dbo.ProjectUsers", new[] { "ProjectId" });
            DropIndex("dbo.ProjectApplicationUsers", new[] { "Project_ModuleId" });
            DropIndex("dbo.ProjectApplicationUsers", new[] { "ApplicationUser_Id" });
            AddColumn("dbo.Projects", "ApplicationUser_Id", c => c.String(maxLength: 128));
            AddColumn("dbo.ProjectUsers", "ProjectPropertyId", c => c.Int());
            AlterColumn("dbo.ProjectUsers", "ProjectId", c => c.String(maxLength: 128));
            CreateIndex("dbo.Projects", "ApplicationUser_Id");
            CreateIndex("dbo.ProjectUsers", "ProjectPropertyId");
            CreateIndex("dbo.ProjectUsers", "ProjectId");
            AddForeignKey("dbo.ProjectUsers", "ProjectPropertyId", "dbo.ProjectProperties", "Id");
            AddForeignKey("dbo.Projects", "ApplicationUser_Id", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.ProjectUsers", "ProjectId", "dbo.Projects", "ModuleId");
        }
        
        public override void Down()
        {            
            DropForeignKey("dbo.ProjectUsers", "ProjectId", "dbo.Projects");
            DropForeignKey("dbo.Projects", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.ProjectUsers", "ProjectPropertyId", "dbo.ProjectProperties");
            DropIndex("dbo.ProjectUsers", new[] { "ProjectId" });
            DropIndex("dbo.ProjectUsers", new[] { "ProjectPropertyId" });
            DropIndex("dbo.Projects", new[] { "ApplicationUser_Id" });
            AlterColumn("dbo.ProjectUsers", "ProjectId", c => c.String(nullable: false, maxLength: 128));
            DropColumn("dbo.ProjectUsers", "ProjectPropertyId");
            DropColumn("dbo.Projects", "ApplicationUser_Id");
            CreateIndex("dbo.ProjectApplicationUsers", "ApplicationUser_Id");
            CreateIndex("dbo.ProjectApplicationUsers", "Project_ModuleId");
            CreateIndex("dbo.ProjectUsers", "ProjectId");
            AddForeignKey("dbo.ProjectUsers", "ProjectId", "dbo.Projects", "ModuleId", cascadeDelete: true);
            AddForeignKey("dbo.ProjectApplicationUsers", "ApplicationUser_Id", "dbo.AspNetUsers", "Id", cascadeDelete: true);
            AddForeignKey("dbo.ProjectApplicationUsers", "Project_ModuleId", "dbo.Projects", "ModuleId", cascadeDelete: true);
        }
    }
}
