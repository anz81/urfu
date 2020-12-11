namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Practice : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PracticeAdmissionCompanys",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PracticeId = c.Int(nullable: false),
                        ContractId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Contracts", t => t.ContractId, cascadeDelete: true)
                .ForeignKey("dbo.Practices", t => t.PracticeId, cascadeDelete: true)
                .Index(t => t.PracticeId)
                .Index(t => t.ContractId);
            
            CreateTable(
                "dbo.Practices",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        StudentId = c.String(maxLength: 128),
                        DisciplineUUID = c.String(),
                        GroupHistoryId = c.String(maxLength: 128),
                        Year = c.Int(nullable: false),
                        SemesterId = c.Int(nullable: false),
                        BeginDate = c.DateTime(),
                        EndDate = c.DateTime(),
                        FinishTheme = c.String(maxLength: 1000),
                        IsExternal = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.GroupsHistory", t => t.GroupHistoryId)
                .ForeignKey("dbo.Semesters", t => t.SemesterId, cascadeDelete: true)
                .ForeignKey("dbo.Students", t => t.StudentId)
                .Index(t => t.StudentId)
                .Index(t => t.GroupHistoryId)
                .Index(t => t.SemesterId);
            
            CreateTable(
                "dbo.PracticeAdmissions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PracticeId = c.Int(nullable: false),
                        TheacherPKey = c.String(maxLength: 127),
                        PracticeThemeId = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Practices", t => t.PracticeId, cascadeDelete: true)
                .ForeignKey("dbo.Teachers", t => t.TheacherPKey)
                .ForeignKey("dbo.PracticeThemes", t => t.PracticeThemeId, cascadeDelete: true)
                .Index(t => t.PracticeId)
                .Index(t => t.TheacherPKey)
                .Index(t => t.PracticeThemeId);
            
            CreateTable(
                "dbo.PracticeThemes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DisciplineUUID = c.String(),
                        Theme = c.String(maxLength: 1000),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PracticeTeachers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DisciplineUUID = c.String(),
                        TheacherPKey = c.String(maxLength: 127),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Teachers", t => t.TheacherPKey)
                .Index(t => t.TheacherPKey);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PracticeTeachers", "TheacherPKey", "dbo.Teachers");
            DropForeignKey("dbo.Practices", "StudentId", "dbo.Students");
            DropForeignKey("dbo.Practices", "SemesterId", "dbo.Semesters");
            DropForeignKey("dbo.Practices", "GroupHistoryId", "dbo.GroupsHistory");
            DropForeignKey("dbo.PracticeAdmissions", "PracticeThemeId", "dbo.PracticeThemes");
            DropForeignKey("dbo.PracticeAdmissions", "TheacherPKey", "dbo.Teachers");
            DropForeignKey("dbo.PracticeAdmissions", "PracticeId", "dbo.Practices");
            DropForeignKey("dbo.PracticeAdmissionCompanys", "PracticeId", "dbo.Practices");
            DropForeignKey("dbo.PracticeAdmissionCompanys", "ContractId", "dbo.Contracts");
            DropIndex("dbo.PracticeTeachers", new[] { "TheacherPKey" });
            DropIndex("dbo.PracticeAdmissions", new[] { "PracticeThemeId" });
            DropIndex("dbo.PracticeAdmissions", new[] { "TheacherPKey" });
            DropIndex("dbo.PracticeAdmissions", new[] { "PracticeId" });
            DropIndex("dbo.Practices", new[] { "SemesterId" });
            DropIndex("dbo.Practices", new[] { "GroupHistoryId" });
            DropIndex("dbo.Practices", new[] { "StudentId" });
            DropIndex("dbo.PracticeAdmissionCompanys", new[] { "ContractId" });
            DropIndex("dbo.PracticeAdmissionCompanys", new[] { "PracticeId" });
            DropTable("dbo.PracticeTeachers");
            DropTable("dbo.PracticeThemes");
            DropTable("dbo.PracticeAdmissions");
            DropTable("dbo.Practices");
            DropTable("dbo.PracticeAdmissionCompanys");
        }
    }
}
