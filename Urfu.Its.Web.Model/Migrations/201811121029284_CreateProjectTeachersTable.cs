namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateProjectTeachersTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ProjectTeachers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProjectPropertyId = c.Int(nullable: false),
                        TeacherId = c.String(maxLength: 127, nullable: false),
                        Type = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ProjectProperties", t => t.ProjectPropertyId, cascadeDelete: true)
                .ForeignKey("dbo.Teachers", t => t.TeacherId)
                .Index(t => t.ProjectPropertyId)
                .Index(t => t.TeacherId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProjectTeachers", "TeacherId", "dbo.Teachers");
            DropForeignKey("dbo.ProjectTeachers", "ProjectPropertyId", "dbo.ProjectProperties");
            DropIndex("dbo.ProjectTeachers", new[] { "TeacherId" });
            DropIndex("dbo.ProjectTeachers", new[] { "ProjectPropertyId" });
            DropTable("dbo.ProjectTeachers");
        }
    }
}
