namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Appload_DisciplineType : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Apploads", "DisciplineType", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Apploads", "DisciplineType");
        }
    }
}
