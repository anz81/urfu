namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateMUPSubgroupTeacherTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MUPSubgroupTeachers",
                c => new
                    {
                        MUPSubgroupId = c.Int(nullable: false),
                        TeacherId = c.String(nullable: false, maxLength: 127),
                    })
                .PrimaryKey(t => new { t.MUPSubgroupId, t.TeacherId })
                .ForeignKey("dbo.MUPSubgroups", t => t.MUPSubgroupId, cascadeDelete: true)
                .ForeignKey("dbo.Teachers", t => t.TeacherId, cascadeDelete: true)
                .Index(t => t.MUPSubgroupId)
                .Index(t => t.TeacherId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MUPSubgroupTeachers", "TeacherId", "dbo.Teachers");
            DropForeignKey("dbo.MUPSubgroupTeachers", "MUPSubgroupId", "dbo.MUPSubgroups");
            DropIndex("dbo.MUPSubgroupTeachers", new[] { "TeacherId" });
            DropIndex("dbo.MUPSubgroupTeachers", new[] { "MUPSubgroupId" });
            DropTable("dbo.MUPSubgroupTeachers");
        }
    }
}
