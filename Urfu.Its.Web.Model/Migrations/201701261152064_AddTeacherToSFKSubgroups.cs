namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTeacherToSFKSubgroups : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SectionFKSubgroups", "TeacherId", c => c.String(maxLength: 127));
            CreateIndex("dbo.SectionFKSubgroups", "TeacherId");
            AddForeignKey("dbo.SectionFKSubgroups", "TeacherId", "dbo.Teachers", "pkey");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SectionFKSubgroups", "TeacherId", "dbo.Teachers");
            DropIndex("dbo.SectionFKSubgroups", new[] { "TeacherId" });
            DropColumn("dbo.SectionFKSubgroups", "TeacherId");
        }
    }
}
