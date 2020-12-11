namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class WorkingProgramAuthorsStringLengthConstraints : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.WorkingProgramAuthors", "LastName", c => c.String(maxLength: 100));
            AlterColumn("dbo.WorkingProgramAuthors", "FirstName", c => c.String(maxLength: 100));
            AlterColumn("dbo.WorkingProgramAuthors", "MiddleName", c => c.String(maxLength: 100));
            AlterColumn("dbo.WorkingProgramAuthors", "AcademicDegree", c => c.String(maxLength: 100));
            AlterColumn("dbo.WorkingProgramAuthors", "AcademicTitle", c => c.String(maxLength: 100));
            AlterColumn("dbo.WorkingProgramAuthors", "Post", c => c.String(maxLength: 100));
            AlterColumn("dbo.WorkingProgramAuthors", "Workplace", c => c.String(maxLength: 100));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.WorkingProgramAuthors", "Workplace", c => c.String());
            AlterColumn("dbo.WorkingProgramAuthors", "Post", c => c.String());
            AlterColumn("dbo.WorkingProgramAuthors", "AcademicTitle", c => c.String());
            AlterColumn("dbo.WorkingProgramAuthors", "AcademicDegree", c => c.String());
            AlterColumn("dbo.WorkingProgramAuthors", "MiddleName", c => c.String());
            AlterColumn("dbo.WorkingProgramAuthors", "FirstName", c => c.String());
            AlterColumn("dbo.WorkingProgramAuthors", "LastName", c => c.String());
        }
    }
}
