namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCreateDate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Variants", "CreateDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Variants", "CreateDate");
        }
    }
}
