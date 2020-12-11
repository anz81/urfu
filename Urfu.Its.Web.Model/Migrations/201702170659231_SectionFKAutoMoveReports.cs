namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SectionFKAutoMoveReports : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SectionFKAutoMoveReports",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        FileName = c.String(),
                        Content = c.Binary(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.SectionFKAutoMoveReports");
        }
    }
}
