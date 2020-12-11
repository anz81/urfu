namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateBasicCharacteristicOPRatifyDataTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BasicCharacteristicOPRatifyDatas",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Year = c.Int(nullable: false),
                        RatifyingPersonPost = c.String(),
                        RatifyingSubdivision = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.BasicCharacteristicOPRatifyDatas");
        }
    }
}
