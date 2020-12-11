namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MissingIndexesImpl : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Apploads", "IX_ApploadDto_eduDiscipline");
            DropIndex("dbo.Apploads", "IX_ApploadDto_discipline");
            DropIndex("dbo.VariantAdmissions", new[] { "studentId" });
            DropIndex("dbo.VariantAdmissions", new[] { "variantId" });
            CreateIndex("dbo.Apploads", new[] { "eduDiscipline", "grp", "dckey", "status" }, name: "IX_ApploadDto_eduDiscipline");
            CreateIndex("dbo.Apploads", new[] { "discipline", "grp", "term", "status" }, name: "IX_ApploadDto_discipline");
            CreateIndex("dbo.VariantAdmissions", new[] { "variantId", "studentId", "Status" }, name: "IX_VariantAdmission_Contents1");
            CreateIndex("dbo.VariantAdmissions", new[] { "studentId", "variantId", "Status" }, name: "IX_VariantAdmission_Contents2");
        }
        
        public override void Down()
        {
            DropIndex("dbo.VariantAdmissions", "IX_VariantAdmission_Contents2");
            DropIndex("dbo.VariantAdmissions", "IX_VariantAdmission_Contents1");
            DropIndex("dbo.Apploads", "IX_ApploadDto_discipline");
            DropIndex("dbo.Apploads", "IX_ApploadDto_eduDiscipline");
            CreateIndex("dbo.VariantAdmissions", "variantId");
            CreateIndex("dbo.VariantAdmissions", "studentId");
            CreateIndex("dbo.Apploads", "discipline", name: "IX_ApploadDto_discipline");
            CreateIndex("dbo.Apploads", new[] { "eduDiscipline", "grp", "dckey" }, name: "IX_ApploadDto_eduDiscipline");
        }
    }
}
