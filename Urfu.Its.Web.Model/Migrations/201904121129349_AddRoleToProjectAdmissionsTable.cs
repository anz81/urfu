namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddRoleToProjectAdmissionsTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProjectAdmissions", "RoleId", c => c.Int());
            CreateIndex("dbo.ProjectAdmissions", "RoleId");
            AddForeignKey("dbo.ProjectAdmissions", "RoleId", "dbo.ProjectRoles", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProjectAdmissions", "RoleId", "dbo.ProjectRoles");
            DropIndex("dbo.ProjectAdmissions", new[] { "RoleId" });
            DropColumn("dbo.ProjectAdmissions", "RoleId");
        }
    }
}
