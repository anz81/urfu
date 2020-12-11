namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Variants_DocumentName : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Variants", "DocumentName", c => c.String(maxLength: 255));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Variants", "DocumentName");
        }
    }
}
