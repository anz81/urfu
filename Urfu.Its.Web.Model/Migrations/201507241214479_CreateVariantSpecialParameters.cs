namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateVariantSpecialParameters : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FamilirizationConditions",
                c => new
                    {
                        Name = c.String(nullable: false, maxLength: 127),
                    })
                .PrimaryKey(t => t.Name);
            
            CreateTable(
                "dbo.FamilirizationTeches",
                c => new
                    {
                        Name = c.String(nullable: false, maxLength: 127),
                    })
                .PrimaryKey(t => t.Name);
            
            CreateTable(
                "dbo.FamilirizationTypes",
                c => new
                    {
                        Name = c.String(nullable: false, maxLength: 127),
                    })
                .PrimaryKey(t => t.Name);
            
            CreateTable(
                "dbo.Qualifications",
                c => new
                    {
                        Name = c.String(nullable: false, maxLength: 127),
                    })
                .PrimaryKey(t => t.Name);
            
            AddColumn("dbo.Variants", "familirizationType", c => c.String());
            AddColumn("dbo.Variants", "familirizationTech", c => c.String());
            AddColumn("dbo.Variants", "familirizationCondition", c => c.String());
            AddColumn("dbo.Variants", "qualification", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Variants", "qualification");
            DropColumn("dbo.Variants", "familirizationCondition");
            DropColumn("dbo.Variants", "familirizationTech");
            DropColumn("dbo.Variants", "familirizationType");
            DropTable("dbo.Qualifications");
            DropTable("dbo.FamilirizationTypes");
            DropTable("dbo.FamilirizationTeches");
            DropTable("dbo.FamilirizationConditions");
        }
    }
}
