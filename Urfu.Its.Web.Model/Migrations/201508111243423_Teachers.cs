namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Teachers : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PlanTeachers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        load = c.String(maxLength: 127),
                        moduleId = c.String(maxLength: 127),
                        eduplanUuid = c.String(maxLength: 127),
                        variantId = c.Int(nullable: false),
                        catalogDisciplineUuid = c.String(maxLength: 127),
                        TeacherPkey = c.String(maxLength: 127),
                        Selectable = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Teachers", t => t.TeacherPkey)
                .Index(t => new { t.variantId, t.moduleId, t.eduplanUuid, t.catalogDisciplineUuid, t.load }, name: "IX_PlanTeacher_Contents")
                .Index(t => t.TeacherPkey);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PlanTeachers", "TeacherPkey", "dbo.Teachers");
            DropIndex("dbo.PlanTeachers", new[] { "TeacherPkey" });
            DropIndex("dbo.PlanTeachers", "IX_PlanTeacher_Contents");
            DropTable("dbo.PlanTeachers");
        }
    }
}
