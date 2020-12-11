namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PracticeWays_Create : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PracticeWays",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.PracticeWays");
        }
    }
}
