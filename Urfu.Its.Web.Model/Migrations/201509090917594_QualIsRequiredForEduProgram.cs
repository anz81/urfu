namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class QualIsRequiredForEduProgram : DbMigration
    {
        public override void Up()
        {
            Sql("delete from eduprograms where qualification is null");
            AlterColumn("dbo.EduPrograms", "qualification", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.EduPrograms", "qualification", c => c.String());
        }
    }
}
