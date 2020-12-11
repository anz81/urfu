namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DropPlanTerms : DbMigration
    {
        public override void Up()
        {
            DropTable("dbo.PlanTerms");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.PlanTerms",
                c => new
                    {
                        disciplineUID = c.String(nullable: false, maxLength: 128),
                        Year = c.Int(nullable: false),
                        TmersCount = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.disciplineUID);
            
        }
    }
}
