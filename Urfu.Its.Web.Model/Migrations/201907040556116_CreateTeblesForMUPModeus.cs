namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateTeblesForMUPModeus : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MUPModeusDirections",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MUPModeusId = c.String(maxLength: 128),
                        DirectionId = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Directions", t => t.DirectionId)
                .ForeignKey("dbo.MUPModeus", t => t.MUPModeusId)
                .Index(t => t.MUPModeusId)
                .Index(t => t.DirectionId);
            
            CreateTable(
                "dbo.MUPModeus",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(),
                        ShortName = c.String(),
                        Course = c.Int(),
                        SemesterId = c.Int(),
                        Units = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Semesters", t => t.SemesterId)
                .Index(t => t.SemesterId);
            
            CreateTable(
                "dbo.MUPModeusRealizations",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        CourseUnitId = c.String(maxLength: 128),
                        Deleted = c.Boolean(nullable: false),
                        State = c.String(),
                        Year = c.Int(),
                        SemesterId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.MUPModeus", t => t.CourseUnitId)
                .ForeignKey("dbo.Semesters", t => t.SemesterId)
                .Index(t => t.CourseUnitId)
                .Index(t => t.SemesterId);
            
            CreateTable(
                "dbo.MUPModeusTeachers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MUPModeusTeamId = c.String(maxLength: 128),
                        TmerId = c.String(maxLength: 128),
                        Deleted = c.Boolean(nullable: false),
                        TeacherId = c.String(),
                        PersonId = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.MUPModeusTeams", t => t.MUPModeusTeamId)
                .ForeignKey("dbo.Tmer", t => t.TmerId)
                .Index(t => t.MUPModeusTeamId)
                .Index(t => t.TmerId);
            
            CreateTable(
                "dbo.MUPModeusTeams",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        MUPModeusRealizationId = c.String(maxLength: 128),
                        MUPModeusId = c.String(maxLength: 128),
                        Year = c.Int(),
                        SemesterId = c.Int(),
                        Name = c.String(),
                        Limit = c.Int(),
                        StudentsCount = c.Int(),
                        Deleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.MUPModeus", t => t.MUPModeusId)
                .ForeignKey("dbo.MUPModeusRealizations", t => t.MUPModeusRealizationId)
                .ForeignKey("dbo.Semesters", t => t.SemesterId)
                .Index(t => t.MUPModeusRealizationId)
                .Index(t => t.MUPModeusId)
                .Index(t => t.SemesterId);
            
            CreateTable(
                "dbo.MUPModeusTeamStudents",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MUPModeusTeamId = c.String(maxLength: 128),
                        StudentId = c.String(),
                        PersonId = c.String(),
                        Deleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.MUPModeusTeams", t => t.MUPModeusTeamId)
                .Index(t => t.MUPModeusTeamId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MUPModeusTeamStudents", "MUPModeusTeamId", "dbo.MUPModeusTeams");
            DropForeignKey("dbo.MUPModeusTeachers", "TmerId", "dbo.Tmer");
            DropForeignKey("dbo.MUPModeusTeachers", "MUPModeusTeamId", "dbo.MUPModeusTeams");
            DropForeignKey("dbo.MUPModeusTeams", "SemesterId", "dbo.Semesters");
            DropForeignKey("dbo.MUPModeusTeams", "MUPModeusRealizationId", "dbo.MUPModeusRealizations");
            DropForeignKey("dbo.MUPModeusTeams", "MUPModeusId", "dbo.MUPModeus");
            DropForeignKey("dbo.MUPModeusRealizations", "SemesterId", "dbo.Semesters");
            DropForeignKey("dbo.MUPModeusRealizations", "CourseUnitId", "dbo.MUPModeus");
            DropForeignKey("dbo.MUPModeusDirections", "MUPModeusId", "dbo.MUPModeus");
            DropForeignKey("dbo.MUPModeus", "SemesterId", "dbo.Semesters");
            DropForeignKey("dbo.MUPModeusDirections", "DirectionId", "dbo.Directions");
            DropIndex("dbo.MUPModeusTeamStudents", new[] { "MUPModeusTeamId" });
            DropIndex("dbo.MUPModeusTeams", new[] { "SemesterId" });
            DropIndex("dbo.MUPModeusTeams", new[] { "MUPModeusId" });
            DropIndex("dbo.MUPModeusTeams", new[] { "MUPModeusRealizationId" });
            DropIndex("dbo.MUPModeusTeachers", new[] { "TmerId" });
            DropIndex("dbo.MUPModeusTeachers", new[] { "MUPModeusTeamId" });
            DropIndex("dbo.MUPModeusRealizations", new[] { "SemesterId" });
            DropIndex("dbo.MUPModeusRealizations", new[] { "CourseUnitId" });
            DropIndex("dbo.MUPModeus", new[] { "SemesterId" });
            DropIndex("dbo.MUPModeusDirections", new[] { "DirectionId" });
            DropIndex("dbo.MUPModeusDirections", new[] { "MUPModeusId" });
            DropTable("dbo.MUPModeusTeamStudents");
            DropTable("dbo.MUPModeusTeams");
            DropTable("dbo.MUPModeusTeachers");
            DropTable("dbo.MUPModeusRealizations");
            DropTable("dbo.MUPModeus");
            DropTable("dbo.MUPModeusDirections");
        }
    }
}
