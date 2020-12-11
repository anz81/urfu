namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserMiorsFix : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.UserMinors", "ModuleId", "dbo.Modules");
            AddForeignKey("dbo.UserMinors", "ModuleId", "dbo.Modules", "uuid");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserMinors", "ModuleId", "dbo.Modules");
            AddForeignKey("dbo.UserMinors", "ModuleId", "dbo.Modules", "uuid", cascadeDelete: true);
        }
    }
}
