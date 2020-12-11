namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AdditionalDisciplineUidinPlans : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Plans", "additionalUUID", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Plans", "additionalUUID");
        }
    }
}
