namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDeadlineToVariantSelectionGroups : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Variants", "SelectionDeadline", c => c.DateTime());
            AddColumn("dbo.VariantSelectionGroups", "SelectionDeadline", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.VariantSelectionGroups", "SelectionDeadline");
            DropColumn("dbo.Variants", "SelectionDeadline");
        }
    }
}
