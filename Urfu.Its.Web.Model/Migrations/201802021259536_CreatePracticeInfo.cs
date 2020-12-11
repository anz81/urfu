namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreatePracticeInfo : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PracticeInfo",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DisciplineUUID = c.String(maxLength: 128),
                        PracticeWayId = c.Int(nullable: false),
                        PracticeTimeId = c.Int(nullable: false),
                        BeginDate = c.DateTime(),
                        EndDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PracticeTimes", t => t.PracticeTimeId, cascadeDelete: true)
                .ForeignKey("dbo.PracticeWays", t => t.PracticeWayId, cascadeDelete: true)
                .Index(t => t.DisciplineUUID, name: "IX_PracticeInfo_DisciplineUUID")
                .Index(t => t.PracticeWayId)
                .Index(t => t.PracticeTimeId);
            
            CreateTable(
                "dbo.PracticeTimes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id);

            Sql(
@"
Insert into dbo.PracticeTimes(Description) values('частично рассредоточенная')
Insert into dbo.PracticeTimes(Description) values('рассредоточенная')
Insert into dbo.PracticeTimes(Description) values('непрерывная')

Insert into dbo.PracticeWays(Description) values('стационарная')
Insert into dbo.PracticeWays(Description) values('выездная')
Insert into dbo.PracticeWays(Description) values('стационарная, выездная')
Insert into dbo.PracticeWays(Description) values('полевая')
");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PracticeInfo", "PracticeWayId", "dbo.PracticeWays");
            DropForeignKey("dbo.PracticeInfo", "PracticeTimeId", "dbo.PracticeTimes");
            DropIndex("dbo.PracticeInfo", new[] { "PracticeTimeId" });
            DropIndex("dbo.PracticeInfo", new[] { "PracticeWayId" });
            DropIndex("dbo.PracticeInfo", "IX_PracticeInfo_DisciplineUUID");
            DropTable("dbo.PracticeTimes");
            DropTable("dbo.PracticeInfo");
        }
    }
}
