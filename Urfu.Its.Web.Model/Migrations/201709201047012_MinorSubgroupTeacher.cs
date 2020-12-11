namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MinorSubgroupTeacher : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MinorSubgroups", "TeacherId", c => c.String(maxLength: 127));
            CreateIndex("dbo.MinorSubgroups", "TeacherId");
            AddForeignKey("dbo.MinorSubgroups", "TeacherId", "dbo.Teachers", "pkey");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MinorSubgroups", "TeacherId", "dbo.Teachers");
            DropIndex("dbo.MinorSubgroups", new[] { "TeacherId" });
            DropColumn("dbo.MinorSubgroups", "TeacherId");
        }
    }
}
