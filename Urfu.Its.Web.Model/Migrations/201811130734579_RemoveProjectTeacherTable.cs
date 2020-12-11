namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveProjectTeacherTable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ProjectTeachers", "ProjectPropertyId", "dbo.ProjectProperties");
            DropForeignKey("dbo.ProjectTeachers", "TeacherId", "dbo.Teachers");
            DropIndex("dbo.ProjectTeachers", new[] { "ProjectPropertyId" });
            DropIndex("dbo.ProjectTeachers", new[] { "TeacherId" });
            DropTable("dbo.ProjectTeachers");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.ProjectTeachers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProjectPropertyId = c.Int(nullable: false),
                        TeacherId = c.String(maxLength: 127),
                        Type = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateIndex("dbo.ProjectTeachers", "TeacherId");
            CreateIndex("dbo.ProjectTeachers", "ProjectPropertyId");
            AddForeignKey("dbo.ProjectTeachers", "TeacherId", "dbo.Teachers", "pkey");
            AddForeignKey("dbo.ProjectTeachers", "ProjectPropertyId", "dbo.ProjectProperties", "Id", cascadeDelete: true);
        }
    }
}
