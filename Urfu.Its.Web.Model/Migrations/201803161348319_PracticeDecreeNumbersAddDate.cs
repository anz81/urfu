namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PracticeDecreeNumbersAddDate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PracticeDecreeNumbers", "DecreeDate", c => c.DateTime());
            AlterColumn("dbo.PracticeDecreeNumbers", "Number", c => c.String(maxLength: 20));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.PracticeDecreeNumbers", "Number", c => c.String());
            DropColumn("dbo.PracticeDecreeNumbers", "DecreeDate");
        }
    }
}
