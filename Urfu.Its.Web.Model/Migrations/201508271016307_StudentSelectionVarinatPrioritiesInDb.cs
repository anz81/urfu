namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class StudentSelectionVarinatPrioritiesInDb : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.StudentVariantSelections",
                c => new
                    {
                        studentId = c.String(nullable: false, maxLength: 128),
                        selectedVariantPriority = c.Int(nullable: false),
                        selectedVariantId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.studentId, t.selectedVariantPriority })
                .ForeignKey("dbo.Students", t => t.studentId)
                .ForeignKey("dbo.Variants", t => t.selectedVariantId)
                .Index(t => t.studentId)
                .Index(t => t.selectedVariantId);
            
            CreateTable(
                "dbo.StudentSelectionPriorities",
                c => new
                    {
                        studentId = c.String(nullable: false, maxLength: 128),
                        selectedVariantPriority = c.Int(nullable: false),
                        variantContentId = c.Int(nullable: false),
                        proprity = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.studentId, t.selectedVariantPriority, t.variantContentId })
                .ForeignKey("dbo.VariantContents", t => t.variantContentId)
                .ForeignKey("dbo.StudentVariantSelections", t => new { t.studentId, t.selectedVariantPriority }, cascadeDelete: true)
                .Index(t => new { t.studentId, t.selectedVariantPriority })
                .Index(t => t.variantContentId);
            
            CreateTable(
                "dbo.StudentSelectionTeachers",
                c => new
                    {
                        studentId = c.String(nullable: false, maxLength: 128),
                        selectedVariantPriority = c.Int(nullable: false),
                        disciplineUUID = c.String(nullable: false, maxLength: 127),
                        control = c.String(nullable: false, maxLength: 127),
                        pkey = c.String(nullable: false, maxLength: 127),
                    })
                .PrimaryKey(t => new { t.studentId, t.selectedVariantPriority, t.disciplineUUID, t.control })
                .ForeignKey("dbo.Teachers", t => t.pkey)
                .ForeignKey("dbo.StudentVariantSelections", t => new { t.studentId, t.selectedVariantPriority }, cascadeDelete: true)
                .Index(t => new { t.studentId, t.selectedVariantPriority })
                .Index(t => t.pkey);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.StudentVariantSelections", "selectedVariantId", "dbo.Variants");
            DropForeignKey("dbo.StudentSelectionTeachers", new[] { "studentId", "selectedVariantPriority" }, "dbo.StudentVariantSelections");
            DropForeignKey("dbo.StudentSelectionTeachers", "pkey", "dbo.Teachers");
            DropForeignKey("dbo.StudentVariantSelections", "studentId", "dbo.Students");
            DropForeignKey("dbo.StudentSelectionPriorities", new[] { "studentId", "selectedVariantPriority" }, "dbo.StudentVariantSelections");
            DropForeignKey("dbo.StudentSelectionPriorities", "variantContentId", "dbo.VariantContents");
            DropIndex("dbo.StudentSelectionTeachers", new[] { "pkey" });
            DropIndex("dbo.StudentSelectionTeachers", new[] { "studentId", "selectedVariantPriority" });
            DropIndex("dbo.StudentSelectionPriorities", new[] { "variantContentId" });
            DropIndex("dbo.StudentSelectionPriorities", new[] { "studentId", "selectedVariantPriority" });
            DropIndex("dbo.StudentVariantSelections", new[] { "selectedVariantId" });
            DropIndex("dbo.StudentVariantSelections", new[] { "studentId" });
            DropTable("dbo.StudentSelectionTeachers");
            DropTable("dbo.StudentSelectionPriorities");
            DropTable("dbo.StudentVariantSelections");
        }
    }
}
