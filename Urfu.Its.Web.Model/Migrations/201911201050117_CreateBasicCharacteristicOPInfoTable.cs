namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateBasicCharacteristicOPInfoTable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.BasicCharacteristicOP", "EduProgramId", "dbo.EduPrograms");
            DropIndex("dbo.BasicCharacteristicOP", new[] { "EduProgramId" });
            CreateTable(
                "dbo.BasicCharacteristicOPInfo",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProfileId = c.String(maxLength: 127),
                        Year = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Profiles", t => t.ProfileId)
                .Index(t => t.ProfileId);
            
            AddColumn("dbo.BasicCharacteristicOP", "BasicCharacteristicOPInfoId", c => c.Int(nullable: false));
            CreateIndex("dbo.BasicCharacteristicOP", "BasicCharacteristicOPInfoId");
            AddForeignKey("dbo.BasicCharacteristicOP", "BasicCharacteristicOPInfoId", "dbo.BasicCharacteristicOPInfo", "Id", cascadeDelete: true);
            DropColumn("dbo.BasicCharacteristicOP", "EduProgramId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.BasicCharacteristicOP", "EduProgramId", c => c.Int(nullable: false));
            DropForeignKey("dbo.BasicCharacteristicOPInfo", "ProfileId", "dbo.Profiles");
            DropForeignKey("dbo.BasicCharacteristicOP", "BasicCharacteristicOPInfoId", "dbo.BasicCharacteristicOPInfo");
            DropIndex("dbo.BasicCharacteristicOP", new[] { "BasicCharacteristicOPInfoId" });
            DropIndex("dbo.BasicCharacteristicOPInfo", new[] { "ProfileId" });
            DropColumn("dbo.BasicCharacteristicOP", "BasicCharacteristicOPInfoId");
            DropTable("dbo.BasicCharacteristicOPInfo");
            CreateIndex("dbo.BasicCharacteristicOP", "EduProgramId");
            AddForeignKey("dbo.BasicCharacteristicOP", "EduProgramId", "dbo.EduPrograms", "Id", cascadeDelete: true);
        }
    }
}
