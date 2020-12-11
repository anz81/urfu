namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddNetworkAttributeToProgram : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EduPrograms", "IsNetwork", c => c.Boolean(nullable: false));
            Sql(@"update EduPrograms set IsNetwork = 1 where exists(select top 1 * from directions d where d.uid=EduPrograms.directionId and okso in ('08.03.01',
'09.03.03',
'13.03.01',
'13.03.02',
'13.03.03',
'15.03.02',
'15.03.04',
'15.03.05',
'22.03.02',
'38.03.01',
'38.03.02',
'38.03.03',
'38.03.04'))");
        }
        
        public override void Down()
        {
            DropColumn("dbo.EduPrograms", "IsNetwork");
        }
    }
}
