namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class VariantAdmission : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.VariantAdmissions",
                c => new
                    {
                        studentId = c.String(nullable: false, maxLength: 128),
                        variantId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.studentId)
                .ForeignKey("dbo.Students", t => t.studentId)
                .ForeignKey("dbo.Variants", t => t.variantId, cascadeDelete: true)
                .Index(t => t.studentId)
                .Index(t => t.variantId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.VariantAdmissions", "variantId", "dbo.Variants");
            DropForeignKey("dbo.VariantAdmissions", "studentId", "dbo.Students");
            DropIndex("dbo.VariantAdmissions", new[] { "variantId" });
            DropIndex("dbo.VariantAdmissions", new[] { "studentId" });
            DropTable("dbo.VariantAdmissions");
        }
    }
}
