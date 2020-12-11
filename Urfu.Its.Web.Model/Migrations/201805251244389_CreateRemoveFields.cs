namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateRemoveFields : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Practices", "remove", c => c.Boolean(nullable: false));
            AddColumn("dbo.PracticeAdmissionCompanys", "remove", c => c.Boolean(nullable: false));
            AddColumn("dbo.PracticeAdmissions", "remove", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PracticeAdmissions", "remove");
            DropColumn("dbo.PracticeAdmissionCompanys", "remove");
            DropColumn("dbo.Practices", "remove");
        }
    }
}
