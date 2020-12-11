namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDivisionToProgram : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EduPrograms", "divisionId", c => c.String());
            AddColumn("dbo.EduPrograms", "Division_uuid", c => c.String(maxLength: 127));
            CreateIndex("dbo.EduPrograms", "Division_uuid");
            AddForeignKey("dbo.EduPrograms", "Division_uuid", "dbo.Divisions", "uuid");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.EduPrograms", "Division_uuid", "dbo.Divisions");
            DropIndex("dbo.EduPrograms", new[] { "Division_uuid" });
            DropColumn("dbo.EduPrograms", "Division_uuid");
            DropColumn("dbo.EduPrograms", "divisionId");
        }
    }
}
