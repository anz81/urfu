namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PracticeDecreeAddSedField : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PracticeDecrees", "DecreeNumber", c => c.String(maxLength: 20));
            AddColumn("dbo.PracticeDecrees", "DecreeDate", c => c.DateTime());
            AddColumn("dbo.PracticeDecrees", "SedId", c => c.Int());
            AddColumn("dbo.PracticeDecrees", "Comment", c => c.String());
            DropColumn("dbo.PracticeDecrees", "Number");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PracticeDecrees", "Number", c => c.String());
            DropColumn("dbo.PracticeDecrees", "Comment");
            DropColumn("dbo.PracticeDecrees", "SedId");
            DropColumn("dbo.PracticeDecrees", "DecreeDate");
            DropColumn("dbo.PracticeDecrees", "DecreeNumber");
        }
    }
}
