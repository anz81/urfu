namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDateOdBirthToPersons : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Persons", "DateOfBirth", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Persons", "DateOfBirth");
        }
    }
}
