namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SectionFKTeachers : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SectionFKTeachers",
                c => new
                    {
                        SectionFKPropertyId = c.Int(nullable: false),
                        TeacherId = c.String(nullable: false, maxLength: 127),
                    })
                .PrimaryKey(t => new { t.SectionFKPropertyId, t.TeacherId })
                .ForeignKey("dbo.SectionFKProperties", t => t.SectionFKPropertyId, cascadeDelete: true)
                .ForeignKey("dbo.Teachers", t => t.TeacherId, cascadeDelete: true)
                .Index(t => t.SectionFKPropertyId)
                .Index(t => t.TeacherId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SectionFKTeachers", "TeacherId", "dbo.Teachers");
            DropForeignKey("dbo.SectionFKTeachers", "SectionFKPropertyId", "dbo.SectionFKProperties");
            DropIndex("dbo.SectionFKTeachers", new[] { "TeacherId" });
            DropIndex("dbo.SectionFKTeachers", new[] { "SectionFKPropertyId" });
            DropTable("dbo.SectionFKTeachers");
        }
    }
}
