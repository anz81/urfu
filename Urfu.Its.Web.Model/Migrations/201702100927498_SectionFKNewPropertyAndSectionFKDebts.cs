namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SectionFKNewPropertyAndSectionFKDebts : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Students", "sectionFKDebtTerms", c => c.String());
            AddColumn("dbo.SectionFKs", "WithoutPriorities", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.SectionFKs", "WithoutPriorities");
            DropColumn("dbo.Students", "sectionFKDebtTerms");
        }
    }
}
