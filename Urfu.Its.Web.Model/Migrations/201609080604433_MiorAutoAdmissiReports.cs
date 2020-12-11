namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MiorAutoAdmissiReports : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MinorAutoAdmissionReports",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false),
                        Content = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.MinorAutoAdmissionReports");
        }
    }
}
