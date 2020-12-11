namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDisciplineMUPFieldToMUPDisciplineConnectionsTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MUPDisciplineConnections", "DisciplineMUPId", c => c.String(maxLength: 128));
            CreateIndex("dbo.MUPDisciplineConnections", "DisciplineMUPId");
            AddForeignKey("dbo.MUPDisciplineConnections", "DisciplineMUPId", "dbo.Disciplines", "uid");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MUPDisciplineConnections", "DisciplineMUPId", "dbo.Disciplines");
            DropIndex("dbo.MUPDisciplineConnections", new[] { "DisciplineMUPId" });
            DropColumn("dbo.MUPDisciplineConnections", "DisciplineMUPId");
        }
    }
}
