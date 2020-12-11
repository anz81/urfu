namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreatePlanDisciplineTerms : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PlanDisciplineTerms",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DisciplineUUID = c.String(maxLength: 128),
                        Term = c.Int(nullable: false),
                        Course = c.Int(nullable: false),
                        SemesterID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.PlanDisciplineTerms");
        }
    }
}
