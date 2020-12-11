namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameMinorAgreementToModuleAgreementTable : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.MinorAgreements", newName: "ModuleAgreements");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.ModuleAgreements", newName: "MinorAgreements");
        }
    }
}
