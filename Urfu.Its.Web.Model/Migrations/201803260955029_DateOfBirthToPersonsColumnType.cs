namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DateOfBirthToPersonsColumnType : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Persons", "DateOfBirth", c => c.DateTime(storeType: "date"));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Persons", "DateOfBirth", c => c.DateTime());
        }
    }
}
