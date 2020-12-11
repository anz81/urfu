namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveProjectTeachersTable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ProjectTeachers", "TeacherId", "dbo.Teachers");
            DropForeignKey("dbo.ProjectTeachers", "ProjectPropertyId", "dbo.ProjectProperties");
            DropIndex("dbo.ProjectTeachers", new[] { "TeacherId" });
            DropIndex("dbo.ProjectTeachers", new[] { "ProjectPropertyId" });
            DropTable("dbo.ProjectTeachers");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.ProjectTeachers",
                c => new
                {
                    ProjectPropertyId = c.Int(nullable: false),
                    TeacherId = c.String(nullable: false, maxLength: 127),
                })
                .PrimaryKey(t => new { t.ProjectPropertyId, t.TeacherId })
                .ForeignKey("dbo.ProjectProperties", t => t.ProjectPropertyId, cascadeDelete: true)
                .ForeignKey("dbo.Teachers", t => t.TeacherId, cascadeDelete: true)
                .Index(t => t.ProjectPropertyId)
                .Index(t => t.TeacherId);
        }
    }
}
