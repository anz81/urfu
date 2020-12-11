namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveTEacherKeyFromProjectPropertyTable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ProjectProperties", "Teacher_pkey", "dbo.Teachers");
            DropIndex("dbo.ProjectProperties", new[] { "Teacher_pkey" });
        }
        
        public override void Down()
        {
            CreateIndex("dbo.ProjectProperties", "Teacher_pkey");
            AddForeignKey("dbo.ProjectProperties", "Teacher_pkey", "dbo.Teachers", "pkey");
        }
    }
}
