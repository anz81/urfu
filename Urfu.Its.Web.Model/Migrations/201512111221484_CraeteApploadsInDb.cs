namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CraeteApploadsInDb : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Apploads",
                c => new
                    {
                        uuid = c.String(nullable: false, maxLength: 256),
                        labSubgroups = c.Int(nullable: false),
                        actionTitle = c.String(),
                        eduDiscipline = c.String(maxLength: 256),
                        duModule = c.String(),
                        lectureFlows = c.String(),
                        chair = c.String(),
                        grp = c.String(),
                        action = c.String(),
                        value = c.Decimal(nullable: false, precision: 18, scale: 2),
                        dckey = c.String(),
                        practiceFlows = c.Int(nullable: false),
                        discipline = c.String(),
                    })
                .PrimaryKey(t => t.uuid)
                .Index(t => t.eduDiscipline, name: "IX_ApploadDto_eduDiscipline");
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.Apploads", "IX_ApploadDto_eduDiscipline");
            DropTable("dbo.Apploads");
        }
    }
}
