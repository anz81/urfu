namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangePrioritiesStructure : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.StudentSelectionPriorities", new[] { "studentId", "selectedVariantPriority" }, "dbo.StudentVariantSelections");
            DropIndex("dbo.StudentSelectionPriorities", new[] { "studentId", "selectedVariantPriority" });
            RenameColumn(table: "dbo.StudentSelectionPriorities", name: "selectedVariantPriority", newName: "StudentVariantSelection_selectedVariantPriority");
            DropPrimaryKey("dbo.StudentSelectionPriorities");
            AddColumn("dbo.StudentSelectionPriorities", "variantId", c => c.Int(nullable: false));
            AddColumn("dbo.StudentSelectionPriorities", "StudentVariantSelection_studentId", c => c.String(maxLength: 128));
            AlterColumn("dbo.StudentSelectionPriorities", "studentId", c => c.String(nullable: false, maxLength: 127));
            AlterColumn("dbo.StudentSelectionPriorities", "StudentVariantSelection_selectedVariantPriority", c => c.Int());
            AddPrimaryKey("dbo.StudentSelectionPriorities", new[] { "studentId", "variantId", "variantContentId" });
            CreateIndex("dbo.StudentSelectionPriorities", new[] { "StudentVariantSelection_studentId", "StudentVariantSelection_selectedVariantPriority" });
            AddForeignKey("dbo.StudentSelectionPriorities", new[] { "StudentVariantSelection_studentId", "StudentVariantSelection_selectedVariantPriority" }, "dbo.StudentVariantSelections", new[] { "studentId", "selectedVariantPriority" });
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.StudentSelectionPriorities", new[] { "StudentVariantSelection_studentId", "StudentVariantSelection_selectedVariantPriority" }, "dbo.StudentVariantSelections");
            DropIndex("dbo.StudentSelectionPriorities", new[] { "StudentVariantSelection_studentId", "StudentVariantSelection_selectedVariantPriority" });
            DropPrimaryKey("dbo.StudentSelectionPriorities");
            AlterColumn("dbo.StudentSelectionPriorities", "StudentVariantSelection_selectedVariantPriority", c => c.Int(nullable: false));
            AlterColumn("dbo.StudentSelectionPriorities", "studentId", c => c.String(nullable: false, maxLength: 128));
            DropColumn("dbo.StudentSelectionPriorities", "StudentVariantSelection_studentId");
            DropColumn("dbo.StudentSelectionPriorities", "variantId");
            AddPrimaryKey("dbo.StudentSelectionPriorities", new[] { "studentId", "selectedVariantPriority", "variantContentId" });
            RenameColumn(table: "dbo.StudentSelectionPriorities", name: "StudentVariantSelection_selectedVariantPriority", newName: "selectedVariantPriority");
            CreateIndex("dbo.StudentSelectionPriorities", new[] { "studentId", "selectedVariantPriority" });
            AddForeignKey("dbo.StudentSelectionPriorities", new[] { "studentId", "selectedVariantPriority" }, "dbo.StudentVariantSelections", new[] { "studentId", "selectedVariantPriority" }, cascadeDelete: true);
        }
    }
}
