namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateDivision : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Divisions",
                c => new
                    {
                        uuid = c.String(nullable: false, maxLength: 127),
                        title = c.String(),
                        typeTitle = c.String(),
                        shortTitle = c.String(),
                        typeCode = c.String(),
                        parent = c.String(),
                    })
                .PrimaryKey(t => t.uuid);
            
            AddColumn("dbo.Plans", "faculty", c => c.String(maxLength: 127));
            CreateIndex("dbo.Plans", "faculty");
            AddForeignKey("dbo.Plans", "faculty", "dbo.Divisions", "uuid");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Plans", "faculty", "dbo.Divisions");
            DropIndex("dbo.Plans", new[] { "faculty" });
            DropColumn("dbo.Plans", "faculty");
            DropTable("dbo.Divisions");
        }
    }
}
