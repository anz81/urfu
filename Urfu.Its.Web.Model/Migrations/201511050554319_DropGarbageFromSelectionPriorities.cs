namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DropGarbageFromSelectionPriorities : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.StudentSelectionPriorities", new[] { "StudentVariantSelection_studentId", "StudentVariantSelection_selectedVariantPriority" }, "dbo.StudentVariantSelections");
            DropForeignKey("dbo.StudentSelectionTeachers", new[] { "studentId", "selectedVariantPriority" }, "dbo.StudentVariantSelections");
            DropIndex("dbo.StudentSelectionPriorities", new[] { "StudentVariantSelection_studentId", "StudentVariantSelection_selectedVariantPriority" });
            DropIndex("dbo.StudentSelectionTeachers", new[] { "studentId", "selectedVariantPriority" });
            DropPrimaryKey("dbo.StudentSelectionTeachers");
            AlterColumn("dbo.StudentSelectionTeachers", "studentId", c => c.String(nullable: false, maxLength: 127));
            AddPrimaryKey("dbo.StudentSelectionTeachers", new[] { "studentId", "selectedVariantPriority", "disciplineUUID", "control" });
            DropColumn("dbo.StudentSelectionPriorities", "StudentVariantSelection_studentId");
            DropColumn("dbo.StudentSelectionPriorities", "StudentVariantSelection_selectedVariantPriority");
        }
        
        public override void Down()
        {
            AddColumn("dbo.StudentSelectionPriorities", "StudentVariantSelection_selectedVariantPriority", c => c.Int());
            AddColumn("dbo.StudentSelectionPriorities", "StudentVariantSelection_studentId", c => c.String(maxLength: 128));
            DropPrimaryKey("dbo.StudentSelectionTeachers");
            AlterColumn("dbo.StudentSelectionTeachers", "studentId", c => c.String(nullable: false, maxLength: 128));
            AddPrimaryKey("dbo.StudentSelectionTeachers", new[] { "studentId", "selectedVariantPriority", "disciplineUUID", "control" });
            CreateIndex("dbo.StudentSelectionTeachers", new[] { "studentId", "selectedVariantPriority" });
            CreateIndex("dbo.StudentSelectionPriorities", new[] { "StudentVariantSelection_studentId", "StudentVariantSelection_selectedVariantPriority" });
            AddForeignKey("dbo.StudentSelectionTeachers", new[] { "studentId", "selectedVariantPriority" }, "dbo.StudentVariantSelections", new[] { "studentId", "selectedVariantPriority" }, cascadeDelete: true);
            AddForeignKey("dbo.StudentSelectionPriorities", new[] { "StudentVariantSelection_studentId", "StudentVariantSelection_selectedVariantPriority" }, "dbo.StudentVariantSelections", new[] { "studentId", "selectedVariantPriority" });
        }
    }
}
