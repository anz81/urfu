namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateEduResultKindTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EduResultKinds",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        ShortName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.EduResults2", "EduResultKindId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.EduResults2", "EduResultKindId");
            DropTable("dbo.EduResultKinds");
        }
    }
}
