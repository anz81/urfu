namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFKPlaces : DbMigration
    {
        public override void Up()
        {

            foreach (var s in data.Split(new[] { Environment.NewLine }, StringSplitOptions.None))
            {
                if (s.Contains(";"))
                {
                    var strings = s.Split(';');
                    Sql("Insert into FirstTrainingPlaceFKs(Address,Description) values ('" + strings[0].Trim() + "','" +
                        strings[1].Trim() + "')");
                }
                else if (s.Contains(":"))
                {
                    var strings = s.Split(':');
                    Sql("Insert into FirstTrainingPlaceFKs(Address,Description) values ('" + strings[1].Trim() + "','" +
                        strings[0].Trim() + "')");
                }

                Sql("delete from [FirstTrainingPlaceFKs] where Exists(select 1 from [FirstTrainingPlaceFKs] x where x.id<[FirstTrainingPlaceFKs].id and x.Address = [FirstTrainingPlaceFKs].Address and x.Description = [FirstTrainingPlaceFKs].Description)");
            }
        }

        private static string data =
            @"ФОК:Фонвизина, 5, 1 эт
зал:Ленина, 51
ФОЦ""Звездный"": Коминтерна,3/1
ФОК: ул. Фонвизина, 5
ул.Мира 29, 1 этаж;
СКИВС:Коминтерна 14, 1 эт
ул.Коминтерна14;гандбольный зал
Мира, 29;лыжная база(подтрибунное помещение)
Мира, 29; фитнес-зал
СКИВС:Коминтерна 14, 1 эт
Фонвизина, 4;зал тайского бокса (вход с торца здания)
Коминтерна, 4; Манеж УрФУ
Малышева 138а; зал тяжелой атлетики
ул.Мира 29;тренажерный зал
;сборная команда";
        
        public override void Down()
        {
        }
    }
}
