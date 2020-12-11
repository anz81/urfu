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
            @"���:���������, 5, 1 ��
���:������, 51
���""��������"": ����������,3/1
���: ��. ���������, 5
��.���� 29, 1 ����;
�����:���������� 14, 1 ��
��.����������14;����������� ���
����, 29;������ ����(������������ ���������)
����, 29; ������-���
�����:���������� 14, 1 ��
���������, 4;��� �������� ����� (���� � ����� ������)
����������, 4; ����� ����
�������� 138�; ��� ������� ��������
��.���� 29;����������� ���
;������� �������";
        
        public override void Down()
        {
        }
    }
}
