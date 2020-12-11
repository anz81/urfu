namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateCompanyLocation : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CompanyLocations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ParentId = c.Int(),
                        Level = c.Int(nullable: false),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.CompanyLocations", t => t.ParentId)
                .Index(t => t.ParentId);
            
            AddColumn("dbo.Companies", "CompanyLocationId", c => c.Int());
            CreateIndex("dbo.Companies", "CompanyLocationId");
            AddForeignKey("dbo.Companies", "CompanyLocationId", "dbo.CompanyLocations", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Companies", "CompanyLocationId", "dbo.CompanyLocations");
            DropForeignKey("dbo.CompanyLocations", "ParentId", "dbo.CompanyLocations");
            DropIndex("dbo.CompanyLocations", new[] { "ParentId" });
            DropIndex("dbo.Companies", new[] { "CompanyLocationId" });
            DropColumn("dbo.Companies", "CompanyLocationId");
            DropTable("dbo.CompanyLocations");
        }
    }
}
