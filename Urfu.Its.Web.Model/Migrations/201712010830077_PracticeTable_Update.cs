namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PracticeTable_Update : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Companies", "Address", c => c.String(maxLength: 255));
            AddColumn("dbo.Companies", "Comment", c => c.String(maxLength: 1024));
            AddColumn("dbo.Contracts", "Comment", c => c.String(maxLength: 1024));
            AddColumn("dbo.Contracts", "Scan", c => c.Binary());
            AlterColumn("dbo.Contracts", "Number", c => c.String());
            AlterColumn("dbo.Contracts", "StartDate", c => c.DateTime());
            AlterColumn("dbo.Contracts", "FinishDate", c => c.DateTime());
            DropColumn("dbo.Companies", "Adddress");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Companies", "Adddress", c => c.String(maxLength: 255));
            AlterColumn("dbo.Contracts", "FinishDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Contracts", "StartDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Contracts", "Number", c => c.Int(nullable: false));
            DropColumn("dbo.Contracts", "Scan");
            DropColumn("dbo.Contracts", "Comment");
            DropColumn("dbo.Companies", "Comment");
            DropColumn("dbo.Companies", "Address");
        }
    }
}
