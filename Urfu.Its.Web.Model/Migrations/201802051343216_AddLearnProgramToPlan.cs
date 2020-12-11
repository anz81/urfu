namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddLearnProgramToPlan : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Plans", "learnProgramUUID", c => c.String());
            AddColumn("dbo.Plans", "learnProgramTitle", c => c.String());
            AddColumn("dbo.Plans", "learnProgramCode", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Plans", "learnProgramCode");
            DropColumn("dbo.Plans", "learnProgramTitle");
            DropColumn("dbo.Plans", "learnProgramUUID");
        }
    }
}
