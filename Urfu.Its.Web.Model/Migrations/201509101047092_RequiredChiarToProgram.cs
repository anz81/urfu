namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RequiredChiarToProgram : DbMigration
    {
        public override void Up()
        {
            Sql("update EduPrograms set chairId = divisionId");
            DropForeignKey("dbo.EduPrograms", "divisionId", "dbo.Divisions");
            DropIndex("dbo.EduPrograms", new[] { "chairId" });
            AlterColumn("dbo.EduPrograms", "chairId", c => c.String(nullable: false, maxLength: 127));
            CreateIndex("dbo.EduPrograms", "chairId");
            AddForeignKey("dbo.EduPrograms", "divisionId", "dbo.Divisions", "uuid");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.EduPrograms", "divisionId", "dbo.Divisions");
            DropIndex("dbo.EduPrograms", new[] { "chairId" });
            AlterColumn("dbo.EduPrograms", "chairId", c => c.String(maxLength: 127));
            CreateIndex("dbo.EduPrograms", "chairId");
            AddForeignKey("dbo.EduPrograms", "divisionId", "dbo.Divisions", "uuid", cascadeDelete: true);
        }
    }
}
