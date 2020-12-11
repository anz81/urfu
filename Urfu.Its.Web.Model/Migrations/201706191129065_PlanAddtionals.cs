namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PlanAddtionals : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PlanAdditionals",
                c => new
                    {
                        eduplanUUID = c.String(nullable: false, maxLength: 128),
                        versionUUID = c.String(nullable: false, maxLength: 128),
                        discipline = c.String(),
                        allload = c.Int(),
                        allaudit = c.Int(),
                        self = c.Int(),
                        lecture = c.Int(),
                        practice = c.Int(),
                        labs = c.Int(),
                        contactTotal = c.Decimal(precision: 6, scale: 2),
                        contactSelf = c.Decimal(precision: 6, scale: 2),
                        contactControl = c.Decimal(precision: 6, scale: 2),
                        contactLecture = c.Decimal(precision: 6, scale: 2),
                        contactPractice = c.Decimal(precision: 6, scale: 2),
                        contactLabs = c.Decimal(precision: 6, scale: 2),
                        tl1 = c.Int(nullable: false),
                        tl2 = c.Int(nullable: false),
                        tl3 = c.Int(nullable: false),
                        tl4 = c.Int(nullable: false),
                        tl5 = c.Int(nullable: false),
                        tl6 = c.Int(nullable: false),
                        tl7 = c.Int(nullable: false),
                        tl8 = c.Int(nullable: false),
                        tl9 = c.Int(nullable: false),
                        tl10 = c.Int(nullable: false),
                        tl11 = c.Int(nullable: false),
                        tl12 = c.Int(nullable: false),
                        gosLoadInTestUnits = c.Int(nullable: false),
                        ttu1 = c.Int(nullable: false),
                        ttu2 = c.Int(nullable: false),
                        ttu3 = c.Int(nullable: false),
                        ttu4 = c.Int(nullable: false),
                        ttu5 = c.Int(nullable: false),
                        ttu6 = c.Int(nullable: false),
                        ttu7 = c.Int(nullable: false),
                        ttu8 = c.Int(nullable: false),
                        ttu9 = c.Int(nullable: false),
                        ttu10 = c.Int(nullable: false),
                        ttu11 = c.Int(nullable: false),
                        ttu12 = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.versionUUID, t.eduplanUUID });
            
        }
        
        public override void Down()
        {
            DropTable("dbo.PlanAdditionals");
        }
    }
}
