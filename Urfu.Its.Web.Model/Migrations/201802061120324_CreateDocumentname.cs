namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateDocumentname : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Companies", "DocumentName", c => c.String(maxLength: 255));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Companies", "DocumentName");
        }
    }
}
