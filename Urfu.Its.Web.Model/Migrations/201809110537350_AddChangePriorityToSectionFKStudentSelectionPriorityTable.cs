namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddChangePriorityToSectionFKStudentSelectionPriorityTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SectionFKStudentSelectionPriorities", "changePriority", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.SectionFKStudentSelectionPriorities", "changePriority");
        }
    }
}
