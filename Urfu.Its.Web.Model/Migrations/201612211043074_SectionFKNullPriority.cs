namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SectionFKNullPriority : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.SectionFKStudentSelectionPriorities", "priority", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.SectionFKStudentSelectionPriorities", "priority", c => c.Int(nullable: false));
        }
    }
}
