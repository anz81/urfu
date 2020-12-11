namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddModuleMUPFieldToMUPDisciplineConnectionTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MUPDisciplineConnections", "ModuleMUPId", c => c.String(maxLength: 128));
            CreateIndex("dbo.MUPDisciplineConnections", "ModuleMUPId");
            AddForeignKey("dbo.MUPDisciplineConnections", "ModuleMUPId", "dbo.Modules", "uuid");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MUPDisciplineConnections", "ModuleMUPId", "dbo.Modules");
            DropIndex("dbo.MUPDisciplineConnections", new[] { "ModuleMUPId" });
            DropColumn("dbo.MUPDisciplineConnections", "ModuleMUPId");
        }
    }
}
