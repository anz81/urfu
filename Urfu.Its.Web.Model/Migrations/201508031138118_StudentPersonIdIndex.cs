namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class StudentPersonIdIndex : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Students", "PersonId", c => c.String(maxLength: 127));
            CreateIndex("dbo.Students", "PersonId", name: "IX_Student_PersonId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Students", "IX_Student_PersonId");
            AlterColumn("dbo.Students", "PersonId", c => c.String());
        }
    }
}
