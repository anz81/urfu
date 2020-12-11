namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PracticeTeacherAndThemeAddColumnYearSemester : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.PracticeTeachers", name: "TheacherPKey", newName: "TeacherPKey");
            RenameIndex(table: "dbo.PracticeTeachers", name: "IX_TheacherPKey", newName: "IX_TeacherPKey");
            AddColumn("dbo.PracticeThemes", "Year", c => c.Int(nullable: false, defaultValue: 2017));
            AddColumn("dbo.PracticeThemes", "SemesterId", c => c.Int(nullable: false, defaultValue: 1));
            AddColumn("dbo.PracticeTeachers", "Year", c => c.Int(nullable: false, defaultValue: 2017));
            AddColumn("dbo.PracticeTeachers", "SemesterId", c => c.Int(nullable: false, defaultValue: 1));
            CreateIndex("dbo.PracticeThemes", "SemesterId");
            CreateIndex("dbo.PracticeTeachers", "SemesterId");
            AddForeignKey("dbo.PracticeThemes", "SemesterId", "dbo.Semesters", "Id", cascadeDelete: false);
            AddForeignKey("dbo.PracticeTeachers", "SemesterId", "dbo.Semesters", "Id", cascadeDelete: false);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PracticeTeachers", "SemesterId", "dbo.Semesters");
            DropForeignKey("dbo.PracticeThemes", "SemesterId", "dbo.Semesters");
            DropIndex("dbo.PracticeTeachers", new[] { "SemesterId" });
            DropIndex("dbo.PracticeThemes", new[] { "SemesterId" });
            DropColumn("dbo.PracticeTeachers", "SemesterId");
            DropColumn("dbo.PracticeTeachers", "Year");
            DropColumn("dbo.PracticeThemes", "SemesterId");
            DropColumn("dbo.PracticeThemes", "Year");
            RenameIndex(table: "dbo.PracticeTeachers", name: "IX_TeacherPKey", newName: "IX_TheacherPKey");
            RenameColumn(table: "dbo.PracticeTeachers", name: "TeacherPKey", newName: "TheacherPKey");
        }
    }
}
