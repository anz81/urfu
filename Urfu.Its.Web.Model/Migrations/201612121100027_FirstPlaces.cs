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

        private static string data = @"�������� ��. ������	:	��. ������
������� ����	:	��.����������,14�
�� ""�������"", ��� ������	:	��.����������,16
�� ""�������"",��� �������	:	��.����������,16, ���.507
��� ����������	:	��.����� �����������,5
��� �����	:	��.��������,138�
��� ����������	:	��.����,29
��� ���  	:	������, 51
��� �������� �����	:	��.���������,4
��� ������� ��������	:	��.��������,138�
����� ����	:	��.����������,4
����-���������� ����	:	��.���������,5
����������	:	��.����,29�, ���.3
������������ ���������	:	��.����,29
�����	:	��.����������,14
������� ����	:	��.����,29
������ �������	:	��.����,29, 1 ��, ����
����������� ���	:	��.����,29
�������, ��. ���������, 4, ���� �������������	:	��.���������,4
���	:	��.���������, 5
���� ""��������""	:	��.����������,3/1
";
        
        public override void Down()
        {
        }
    }
}
