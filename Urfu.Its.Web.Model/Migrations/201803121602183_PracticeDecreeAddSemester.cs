namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PracticeDecreeAddSemester : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PracticeDecrees", "SemesterID", c => c.Int());
            AddColumn("dbo.PracticeDecrees", "Term", c => c.Int());
            AlterColumn("dbo.Practices", "DisciplineUUID", c => c.String(maxLength: 128));
            AlterColumn("dbo.PracticeThemes", "DisciplineUUID", c => c.String(maxLength: 128));
            AlterColumn("dbo.PracticeDecrees", "DisciplineUUID", c => c.String(maxLength: 128));
            AlterColumn("dbo.PracticeTeachers", "DisciplineUUID", c => c.String(maxLength: 128));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.PracticeTeachers", "DisciplineUUID", c => c.String());
            AlterColumn("dbo.PracticeDecrees", "DisciplineUUID", c => c.String());
            AlterColumn("dbo.PracticeThemes", "DisciplineUUID", c => c.String());
            AlterColumn("dbo.Practices", "DisciplineUUID", c => c.String());
            DropColumn("dbo.PracticeDecrees", "Term");
            DropColumn("dbo.PracticeDecrees", "SemesterID");
        }
    }
}
