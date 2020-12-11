namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeKeyInModuleAgreementTable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ModuleAgreements", "ModuleUUID", "dbo.Modules");
            DropIndex("dbo.ModuleAgreements", new[] { "ModuleUUID" });
            DropPrimaryKey("dbo.ModuleAgreements");
            AlterColumn("dbo.ModuleAgreements", "UniId", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.ModuleAgreements", "ModuleUUID", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.ModuleAgreements", "DisciplineUUID", c => c.String(nullable: false, maxLength: 128));
            AddPrimaryKey("dbo.ModuleAgreements", new[] { "ModuleUUID", "DisciplineUUID", "UniId" });
            CreateIndex("dbo.ModuleAgreements", "ModuleUUID");
            AddForeignKey("dbo.ModuleAgreements", "ModuleUUID", "dbo.Modules", "uuid", cascadeDelete: true);
            DropColumn("dbo.ModuleAgreements", "Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ModuleAgreements", "Id", c => c.Int(nullable: false, identity: true));
            DropForeignKey("dbo.ModuleAgreements", "ModuleUUID", "dbo.Modules");
            DropIndex("dbo.ModuleAgreements", new[] { "ModuleUUID" });
            DropPrimaryKey("dbo.ModuleAgreements");
            AlterColumn("dbo.ModuleAgreements", "DisciplineUUID", c => c.String());
            AlterColumn("dbo.ModuleAgreements", "ModuleUUID", c => c.String(maxLength: 128));
            AlterColumn("dbo.ModuleAgreements", "UniId", c => c.String());
            AddPrimaryKey("dbo.ModuleAgreements", "Id");
            CreateIndex("dbo.ModuleAgreements", "ModuleUUID");
            AddForeignKey("dbo.ModuleAgreements", "ModuleUUID", "dbo.Modules", "uuid");
        }
    }
}
