namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreatePracticeDecreeNumbers : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PracticeDecreeNumbers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Year = c.Int(nullable: false),
                        Number = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.PracticeDecreeNumbers");
        }
    }
}
