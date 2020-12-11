namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Plans_additionalType : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Plans", "additionalType", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Plans", "additionalType");
        }
    }
}
