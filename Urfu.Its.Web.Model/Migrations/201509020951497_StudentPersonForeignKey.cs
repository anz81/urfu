namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class StudentPersonForeignKey : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Students", "IX_Student_PersonId");
            AlterColumn("dbo.Students", "PersonId", c => c.String(maxLength: 128));
            CreateIndex("dbo.Students", "PersonId", name: "IX_Student_PersonId");
            AddForeignKey("dbo.Students", "PersonId", "dbo.Persons", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Students", "PersonId", "dbo.Persons");
            DropIndex("dbo.Students", "IX_Student_PersonId");
            AlterColumn("dbo.Students", "PersonId", c => c.String(maxLength: 127));
            CreateIndex("dbo.Students", "PersonId", name: "IX_Student_PersonId");
        }
    }
}
