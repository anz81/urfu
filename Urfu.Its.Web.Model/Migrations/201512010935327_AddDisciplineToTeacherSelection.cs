namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDisciplineToTeacherSelection : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.StudentSelectionTeachers");
            AlterColumn("dbo.StudentSelectionTeachers", "disciplineUUID", c => c.String(nullable: false, maxLength: 128));
            AddPrimaryKey("dbo.StudentSelectionTeachers", new[] { "studentId", "selectedVariantPriority", "disciplineUUID", "control" });
            CreateIndex("dbo.StudentSelectionTeachers", "disciplineUUID");
            AddForeignKey("dbo.StudentSelectionTeachers", "disciplineUUID", "dbo.Disciplines", "uid", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.StudentSelectionTeachers", "disciplineUUID", "dbo.Disciplines");
            DropIndex("dbo.StudentSelectionTeachers", new[] { "disciplineUUID" });
            DropPrimaryKey("dbo.StudentSelectionTeachers");
            AlterColumn("dbo.StudentSelectionTeachers", "disciplineUUID", c => c.String(nullable: false, maxLength: 127));
            AddPrimaryKey("dbo.StudentSelectionTeachers", new[] { "studentId", "selectedVariantPriority", "disciplineUUID", "control" });
        }
    }
}
