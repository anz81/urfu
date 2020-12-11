namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateTrainingDurationTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TrainingDurations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DivisionUuid = c.String(maxLength: 127),
                        Qualification = c.String(),
                        DirectionUid = c.String(maxLength: 128),
                        FamilirizationType = c.String(),
                        Duration = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Directions", t => t.DirectionUid)
                .ForeignKey("dbo.Divisions", t => t.DivisionUuid)
                .Index(t => t.DivisionUuid)
                .Index(t => t.DirectionUid);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TrainingDurations", "DivisionUuid", "dbo.Divisions");
            DropForeignKey("dbo.TrainingDurations", "DirectionUid", "dbo.Directions");
            DropIndex("dbo.TrainingDurations", new[] { "DirectionUid" });
            DropIndex("dbo.TrainingDurations", new[] { "DivisionUuid" });
            DropTable("dbo.TrainingDurations");
        }
    }
}
