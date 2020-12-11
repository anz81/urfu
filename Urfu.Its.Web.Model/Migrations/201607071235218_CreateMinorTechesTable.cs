namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateMinorTechesTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MinorTeches",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Name = c.String(maxLength: 127),
                    })
                .PrimaryKey(t => t.Id);

            Sql($"Insert into MinorTeches(Id, Name)values(1, N'Традиционная')");
            Sql($"Insert into MinorTeches(Id, Name)values(2, N'Смешанная ')");
            Sql($"Insert into MinorTeches(Id, Name)values(3, N'Онлайн курс')");
        }
        
        public override void Down()
        {
            DropTable("dbo.MinorTeches");
        }
    }
}
