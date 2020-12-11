namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AdmissionStatus : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ModuleAdmissions", "Status", c => c.Int(nullable: false));
            AddColumn("dbo.VariantAdmissions", "Status", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.VariantAdmissions", "Status");
            DropColumn("dbo.ModuleAdmissions", "Status");
        }
    }
}
