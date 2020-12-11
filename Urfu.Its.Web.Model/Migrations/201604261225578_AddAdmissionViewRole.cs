namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAdmissionViewRole : DbMigration
    {
        public override void Up()
        {
            //Sql("delete from AspNetRoles where RoleId is null");
        }
        
        public override void Down()
        {
        }
    }
}
