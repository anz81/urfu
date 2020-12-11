namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameSectionFKLimits2SectionFKProperties : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.SectionFKLimits", newName: "SectionFKProperties");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.SectionFKProperties", newName: "SectionFKLimits");
        }
    }
}
