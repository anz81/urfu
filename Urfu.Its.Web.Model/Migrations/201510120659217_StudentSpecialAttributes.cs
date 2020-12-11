namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class StudentSpecialAttributes : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Students", "IsTarget", c => c.Boolean(nullable: false));
            AddColumn("dbo.Students", "IsInternational", c => c.Boolean(nullable: false));
            AddColumn("dbo.Students", "Compensation", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Students", "Compensation");
            DropColumn("dbo.Students", "IsInternational");
            DropColumn("dbo.Students", "IsTarget");
        }
    }
}
