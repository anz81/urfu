namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateProjectRolesTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ProjectRoles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProjectId = c.String(maxLength: 128),
                        Title = c.String(),
                        Description = c.String(),
                        EmployersId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Projects", t => t.ProjectId)
                .Index(t => t.ProjectId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProjectRoles", "ProjectId", "dbo.Projects");
            DropIndex("dbo.ProjectRoles", new[] { "ProjectId" });
            DropTable("dbo.ProjectRoles");
        }
    }
}
