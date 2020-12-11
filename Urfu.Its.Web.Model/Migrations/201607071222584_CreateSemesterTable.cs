namespace Urfu.Its.Web.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class CreateSemesterTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Semesters",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Name = c.String(maxLength: 127),
                    })
                .PrimaryKey(t => t.Id);

            Sql($"Insert into Semesters(Id, Name)values(0, N'Прочий')");
            Sql($"Insert into Semesters(Id, Name)values(1, N'Осенний')");
            Sql($"Insert into Semesters(Id, Name)values(2, N'Весенний')");
        }

        public override void Down()
        {
            DropTable("dbo.Semesters");
        }
    }
}
