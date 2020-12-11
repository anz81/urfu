namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SomeFixesInPrograms : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.EduPrograms", new[] { "Division_uuid" });
            DropColumn("dbo.EduPrograms", "divisionId");
            RenameColumn(table: "dbo.EduPrograms", name: "Division_uuid", newName: "divisionId");
            AlterColumn("dbo.EduPrograms", "divisionId", c => c.String(maxLength: 127));
            CreateIndex("dbo.EduPrograms", "divisionId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.EduPrograms", new[] { "divisionId" });
            AlterColumn("dbo.EduPrograms", "divisionId", c => c.String());
            RenameColumn(table: "dbo.EduPrograms", name: "divisionId", newName: "Division_uuid");
            AddColumn("dbo.EduPrograms", "divisionId", c => c.String());
            CreateIndex("dbo.EduPrograms", "Division_uuid");
        }
    }
}
