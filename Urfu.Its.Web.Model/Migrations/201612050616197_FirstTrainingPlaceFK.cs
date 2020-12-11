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

@"����, 29;������-��� 
����������, 4;����� ���� 
��. ���������, 5; ���
��������, 138�; ��� ������� �������� 
��.����������, 14; ����������� ��� 
����, 29�, �.3;
�.����������� 5, 1 ��;��� 
���������� 16, 7 ����;��� ������ (��� ��. 716) 
�������� 138�;��� �����";


        public override void Down()
        {
            DropTable("dbo.FirstTrainingPlaceFKs");
        }
    }
}
