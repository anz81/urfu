namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RecreatePlanTerms : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PlanTerms",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        disciplineUID = c.String(maxLength: 127),
                        Year = c.Int(nullable: false),
                        TermsCount = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.disciplineUID, name: "IX_PlanTerms_QueryIndex");
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.PlanTerms", "IX_PlanTerms_QueryIndex");
            DropTable("dbo.PlanTerms");
        }
    }
}
