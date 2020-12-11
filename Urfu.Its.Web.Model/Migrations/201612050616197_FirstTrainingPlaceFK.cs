namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FirstTrainingPlaceFK : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FirstTrainingPlaceFKs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Address = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            foreach (var s in data.Split(new [] { Environment.NewLine },StringSplitOptions.None))
            {
                var strings = s.Split(';');
                Sql("Insert into FirstTrainingPlaceFKs(Address,Description) values ('"+strings[0].Trim()+"','" + strings[1].Trim() + "')");
            }
        }

        private static string data =

@"Мира, 29;Фитнес-зал 
Коминтерна, 4;Манеж УрФУ 
ул. Фонвизина, 5; ФОК
Малышева, 138а; Зал тяжелой атлетики 
ул.Коминтерна, 14; Гандбольный зал 
Мира, 29А, к.3;
С.Ковалевской 5, 1 эт;ТЭФ 
Коминтерна 16, 7 этаж;зал борьбы (или оф. 716) 
Малышева 138а;зал бокса";


        public override void Down()
        {
            DropTable("dbo.FirstTrainingPlaceFKs");
        }
    }
}
