namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddQualificationFieldToAreaEducationOrdersTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AreaEducationOrders", "QualificationName", c => c.String(maxLength: 127));
            CreateIndex("dbo.AreaEducationOrders", "QualificationName");
            AddForeignKey("dbo.AreaEducationOrders", "QualificationName", "dbo.Qualifications", "Name");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AreaEducationOrders", "QualificationName", "dbo.Qualifications");
            DropIndex("dbo.AreaEducationOrders", new[] { "QualificationName" });
            DropColumn("dbo.AreaEducationOrders", "QualificationName");
        }
    }
}
