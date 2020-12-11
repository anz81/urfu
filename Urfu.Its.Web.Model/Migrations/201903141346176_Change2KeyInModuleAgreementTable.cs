namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Change2KeyInModuleAgreementTable : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.ModuleAgreements");
            AddPrimaryKey("dbo.ModuleAgreements", new[] { "ModuleUUID", "DisciplineUUID", "UniId", "SemesterId", "EduYear" });
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.ModuleAgreements");
            AddPrimaryKey("dbo.ModuleAgreements", new[] { "ModuleUUID", "DisciplineUUID", "UniId" });
        }
    }
}
