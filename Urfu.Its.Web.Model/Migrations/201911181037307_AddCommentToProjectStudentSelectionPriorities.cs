namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCommentToProjectStudentSelectionPriorities : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProjectStudentSelectionPriorities", "Comment", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProjectStudentSelectionPriorities", "Comment");
        }
    }
}
