namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PracticeAdmissionAddColumns : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.PracticeAdmissions", name: "TheacherPKey", newName: "TeacherPKey");
            RenameColumn(table: "dbo.PracticeAdmissions", name: "TheacherPKey2", newName: "TeacherPKey2");
            RenameIndex(table: "dbo.PracticeAdmissions", name: "IX_TheacherPKey", newName: "IX_TeacherPKey");
            RenameIndex(table: "dbo.PracticeAdmissions", name: "IX_TheacherPKey2", newName: "IX_TeacherPKey2");
            AddColumn("dbo.Practices", "ExternalBeginDate", c => c.DateTime());
            AddColumn("dbo.Practices", "ExternalEndDate", c => c.DateTime());
            AddColumn("dbo.PracticeAdmissions", "ReasonOfDeny", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.PracticeAdmissions", "ReasonOfDeny");
            DropColumn("dbo.Practices", "ExternalEndDate");
            DropColumn("dbo.Practices", "ExternalBeginDate");
            RenameIndex(table: "dbo.PracticeAdmissions", name: "IX_TeacherPKey2", newName: "IX_TheacherPKey2");
            RenameIndex(table: "dbo.PracticeAdmissions", name: "IX_TeacherPKey", newName: "IX_TheacherPKey");
            RenameColumn(table: "dbo.PracticeAdmissions", name: "TeacherPKey2", newName: "TheacherPKey2");
            RenameColumn(table: "dbo.PracticeAdmissions", name: "TeacherPKey", newName: "TheacherPKey");
        }
    }
}
