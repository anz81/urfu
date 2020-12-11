namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddChiarToProgram : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EduPrograms", "chairId", c => c.String(maxLength: 127));
            CreateIndex("dbo.EduPrograms", "chairId");
            AddForeignKey("dbo.EduPrograms", "chairId", "dbo.Divisions", "uuid");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.EduPrograms", "chairId", "dbo.Divisions");
            DropIndex("dbo.EduPrograms", new[] { "chairId" });
            DropColumn("dbo.EduPrograms", "chairId");
        }
    }
}
