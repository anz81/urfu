namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RequisiteOrderFGOS_Date2OnlyDateFormat : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.RequisitesOrdersFGOS", "Date", c => c.DateTime(nullable: false, storeType: "date"));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.RequisitesOrdersFGOS", "Date", c => c.DateTime(nullable: false));
        }
    }
}
