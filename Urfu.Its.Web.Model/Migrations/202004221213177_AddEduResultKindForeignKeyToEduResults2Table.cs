namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddEduResultKindForeignKeyToEduResults2Table : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.EduResults2", "EduResultKindId");
            AddForeignKey("dbo.EduResults2", "EduResultKindId", "dbo.EduResultKinds", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.EduResults2", "EduResultKindId", "dbo.EduResultKinds");
            DropIndex("dbo.EduResults2", new[] { "EduResultKindId" });
        }
    }
}
