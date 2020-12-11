namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CompositeAdmissionKey : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ModuleAdmissions", "moduleId", "dbo.Modules");
            DropForeignKey("dbo.ModuleAdmissions", "studentId", "dbo.Students");
            DropForeignKey("dbo.VariantAdmissions", "studentId", "dbo.Students");
            DropIndex("dbo.ModuleAdmissions", new[] { "moduleId" });
            DropPrimaryKey("dbo.ModuleAdmissions");
            DropPrimaryKey("dbo.VariantAdmissions");
            AlterColumn("dbo.ModuleAdmissions", "moduleId", c => c.String(nullable: false, maxLength: 128));
            AddPrimaryKey("dbo.ModuleAdmissions", new[] { "studentId", "moduleId" });
            AddPrimaryKey("dbo.VariantAdmissions", new[] { "studentId", "variantId" });
            CreateIndex("dbo.ModuleAdmissions", "moduleId");
            AddForeignKey("dbo.ModuleAdmissions", "moduleId", "dbo.Modules", "uuid", cascadeDelete: true);
            AddForeignKey("dbo.ModuleAdmissions", "studentId", "dbo.Students", "Id", cascadeDelete: true);
            AddForeignKey("dbo.VariantAdmissions", "studentId", "dbo.Students", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.VariantAdmissions", "studentId", "dbo.Students");
            DropForeignKey("dbo.ModuleAdmissions", "studentId", "dbo.Students");
            DropForeignKey("dbo.ModuleAdmissions", "moduleId", "dbo.Modules");
            DropIndex("dbo.ModuleAdmissions", new[] { "moduleId" });
            DropPrimaryKey("dbo.VariantAdmissions");
            DropPrimaryKey("dbo.ModuleAdmissions");
            AlterColumn("dbo.ModuleAdmissions", "moduleId", c => c.String(maxLength: 128));
            AddPrimaryKey("dbo.VariantAdmissions", "studentId");
            AddPrimaryKey("dbo.ModuleAdmissions", "studentId");
            CreateIndex("dbo.ModuleAdmissions", "moduleId");
            AddForeignKey("dbo.VariantAdmissions", "studentId", "dbo.Students", "Id");
            AddForeignKey("dbo.ModuleAdmissions", "studentId", "dbo.Students", "Id");
            AddForeignKey("dbo.ModuleAdmissions", "moduleId", "dbo.Modules", "uuid");
        }
    }
}
