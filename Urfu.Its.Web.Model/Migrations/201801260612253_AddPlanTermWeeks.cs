namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPlanTermWeeks : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PlanTermWeeks",
                c => new
                    {
                        eduplanUUID = c.String(nullable: false, maxLength: 128),
                        Term = c.Int(nullable: false),
                        WeeksCount = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.eduplanUUID, t.Term });
            
        }
        
        public override void Down()
        {
            DropTable("dbo.PlanTermWeeks");
        }
    }
}
