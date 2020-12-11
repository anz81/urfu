namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class StudentVariantSelectionPKChange : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.StudentVariantSelections");
            AddPrimaryKey("dbo.StudentVariantSelections", new[] { "studentId", "selectedVariantId" });
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.StudentVariantSelections");
            AddPrimaryKey("dbo.StudentVariantSelections", new[] { "studentId", "selectedVariantPriority" });
        }
    }
}
