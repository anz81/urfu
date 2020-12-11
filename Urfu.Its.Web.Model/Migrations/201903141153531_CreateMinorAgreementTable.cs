namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateMinorAgreementTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MinorAgreements",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UniId = c.String(),
                        ModuleUUID = c.String(maxLength: 128),
                        DisciplineUUID = c.String(),
                        CourseTitle = c.String(),
                        CourseType = c.String(),
                        EduYear = c.Int(nullable: false),
                        StartDate = c.DateTime(),
                        EndDate = c.DateTime(),
                        SemesterId = c.Int(nullable: false),
                        URFUInfoURL = c.String(),
                        CourseURL = c.String(),
                        UniversityTitle = c.String(),
                        UniversityShortTitle = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Modules", t => t.ModuleUUID)
                .ForeignKey("dbo.Semesters", t => t.SemesterId, cascadeDelete: true)
                .Index(t => t.ModuleUUID)
                .Index(t => t.SemesterId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MinorAgreements", "SemesterId", "dbo.Semesters");
            DropForeignKey("dbo.MinorAgreements", "ModuleUUID", "dbo.Modules");
            DropIndex("dbo.MinorAgreements", new[] { "SemesterId" });
            DropIndex("dbo.MinorAgreements", new[] { "ModuleUUID" });
            DropTable("dbo.MinorAgreements");
        }
    }
}
