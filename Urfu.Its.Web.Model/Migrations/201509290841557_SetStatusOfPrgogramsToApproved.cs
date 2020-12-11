namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SetStatusOfPrgogramsToApproved : DbMigration
    {
        public override void Up()
        {
            Sql("update EduPrograms set State = 2");
        }
        
        public override void Down()
        {
        }
    }
}
