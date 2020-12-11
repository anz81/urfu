namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PlansAdditionalType127 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Plans", "additionalType", c => c.String(maxLength: 127));
            CreateIndex("dbo.Plans", "additionalType", name: "IX_Plan_AdditionalType");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Plans", "IX_Plan_AdditionalType");
            AlterColumn("dbo.Plans", "additionalType", c => c.String());
        }
    }
}
