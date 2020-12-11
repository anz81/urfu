namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Practice_AddTeacher2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PracticeAdmissions", "TheacherPKey2", c => c.String(maxLength: 127));
            CreateIndex("dbo.PracticeAdmissions", "TheacherPKey2");
            AddForeignKey("dbo.PracticeAdmissions", "TheacherPKey2", "dbo.Teachers", "pkey");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PracticeAdmissions", "TheacherPKey2", "dbo.Teachers");
            DropIndex("dbo.PracticeAdmissions", new[] { "TheacherPKey2" });
            DropColumn("dbo.PracticeAdmissions", "TheacherPKey2");
        }
    }
}
