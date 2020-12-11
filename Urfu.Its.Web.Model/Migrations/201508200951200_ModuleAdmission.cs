namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ModuleAdmission : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ModuleAdmissions",
                c => new
                    {
                        studentId = c.String(nullable: false, maxLength: 128),
                        moduleId = c.String(maxLength: 128),
                        Published = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.studentId)
                .ForeignKey("dbo.Modules", t => t.moduleId)
                .ForeignKey("dbo.Students", t => t.studentId)
                .Index(t => t.studentId)
                .Index(t => t.moduleId);
            
            AddColumn("dbo.VariantAdmissions", "Published", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ModuleAdmissions", "studentId", "dbo.Students");
            DropForeignKey("dbo.ModuleAdmissions", "moduleId", "dbo.Modules");
            DropIndex("dbo.ModuleAdmissions", new[] { "moduleId" });
            DropIndex("dbo.ModuleAdmissions", new[] { "studentId" });
            DropColumn("dbo.VariantAdmissions", "Published");
            DropTable("dbo.ModuleAdmissions");
        }
    }
}
