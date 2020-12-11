namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OwnershipTypeTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.OwnershipTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FullName = c.String(),
                        ShortName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Companies", "ShortName", c => c.String(maxLength: 255));
            AddColumn("dbo.Companies", "OwnershipTypeId", c => c.Int());
            CreateIndex("dbo.Companies", "OwnershipTypeId");
            AddForeignKey("dbo.Companies", "OwnershipTypeId", "dbo.OwnershipTypes", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Companies", "OwnershipTypeId", "dbo.OwnershipTypes");
            DropIndex("dbo.Companies", new[] { "OwnershipTypeId" });
            DropColumn("dbo.Companies", "OwnershipTypeId");
            DropColumn("dbo.Companies", "ShortName");
            DropTable("dbo.OwnershipTypes");
        }
    }
}
