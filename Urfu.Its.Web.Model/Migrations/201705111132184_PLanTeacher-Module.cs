namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PLanTeacherModule : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.PlanTeachers", "IX_PlanTeacher_Contents");
            AlterColumn("dbo.PlanTeachers", "moduleId", c => c.String(maxLength: 128));
            CreateIndex("dbo.PlanTeachers", new[] { "variantId", "moduleId", "eduplanUuid", "catalogDisciplineUuid", "load" }, name: "IX_PlanTeacher_Contents");
            AddForeignKey("dbo.PlanTeachers", "moduleId", "dbo.Modules", "uuid");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PlanTeachers", "moduleId", "dbo.Modules");
            DropIndex("dbo.PlanTeachers", "IX_PlanTeacher_Contents");
            AlterColumn("dbo.PlanTeachers", "moduleId", c => c.String(maxLength: 127));
            CreateIndex("dbo.PlanTeachers", new[] { "variantId", "moduleId", "eduplanUuid", "catalogDisciplineUuid", "load" }, name: "IX_PlanTeacher_Contents");
        }
    }
}
