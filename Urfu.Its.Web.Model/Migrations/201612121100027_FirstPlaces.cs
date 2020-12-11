namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FirstPlaces : DbMigration
    {
        public override void Up()
        {
            Sql("delete from [FirstTrainingPlaceFKs]");
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

        private static string data = @"Лесопарк оз. Шарташ	:	оз. Шарташ
Бассейн УрФУ	:	ул.Коминтерна,14а
БЦ ""Ельцина"", зал борьбы	:	ул.Коминтерна,16
БЦ ""Ельцина"",зал фитнеса	:	ул.Коминтерна,16, ауд.507
Зал баскетбола	:	ул.Софьи Ковалевской,5
Зал бокса	:	ул.Малышева,138а
Зал гимнастики	:	ул.Мира,29
Зал ОФП  	:	Ленина, 51
Зал тайского бокса	:	ул.Фонвизина,4
Зал тяжелой атлетики	:	ул.Малышева,138а
Манеж УрФУ	:	ул.Коминтерна,4
Мини-футбольное поле	:	ул.Фонвизина,5
Автосервис	:	ул.Мира,29а, каб.3
Подтрибунное помещение	:	ул.Мира,29
СКИВС	:	ул.Коминтерна,14
Стадион УрФУ	:	ул.Мира,29
Корпус ИФКСиМП	:	ул.Мира,29, 1 эт, холл
ТРЕНАЖЕРНЫЙ ЗАЛ	:	ул.Мира,29
Турклуб, ул. Тургенева, 4, парк Харитоновский	:	ул.Тургенева,4
ФОК	:	ул.Фонвизина, 5
ФОСЦ ""Звездный""	:	ул.Коминтерна,3/1
";
        
        public override void Down()
        {
        }
    }
}
