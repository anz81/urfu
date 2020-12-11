namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MinorSubgroupMemberships : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MinorSubgroupMemberships",
                c => new
                    {
                        SubgroupId = c.Int(nullable: false),
                        studentId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.SubgroupId, t.studentId })
                .ForeignKey("dbo.Students", t => t.studentId, cascadeDelete: true)
                .ForeignKey("dbo.MinorSubgroups", t => t.SubgroupId, cascadeDelete: true)
                .Index(t => t.SubgroupId)
                .Index(t => t.studentId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MinorSubgroupMemberships", "SubgroupId", "dbo.MinorSubgroups");
            DropForeignKey("dbo.MinorSubgroupMemberships", "studentId", "dbo.Students");
            DropIndex("dbo.MinorSubgroupMemberships", new[] { "studentId" });
            DropIndex("dbo.MinorSubgroupMemberships", new[] { "SubgroupId" });
            DropTable("dbo.MinorSubgroupMemberships");
        }
    }
}
