namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MinorSubgroups : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MinorSubgroups",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Limit = c.Int(nullable: false),
                        InnerNumber = c.Int(nullable: false),
                        ParentId = c.Int(),
                        MetaSubgroupId = c.Int(nullable: false),
                        ExpectedChildCount = c.Double(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.MinorDisciplineTmerPeriods", t => t.MetaSubgroupId, cascadeDelete: true)
                .ForeignKey("dbo.MinorSubgroups", t => t.ParentId)
                .Index(t => t.ParentId)
                .Index(t => t.MetaSubgroupId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MinorSubgroups", "ParentId", "dbo.MinorSubgroups");
            DropForeignKey("dbo.MinorSubgroups", "MetaSubgroupId", "dbo.MinorDisciplineTmerPeriods");
            DropIndex("dbo.MinorSubgroups", new[] { "MetaSubgroupId" });
            DropIndex("dbo.MinorSubgroups", new[] { "ParentId" });
            DropTable("dbo.MinorSubgroups");
        }
    }
}
