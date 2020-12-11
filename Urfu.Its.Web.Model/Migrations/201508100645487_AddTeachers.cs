namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTeachers : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Teachers",
                c => new
                    {
                        pkey = c.String(nullable: false, maxLength: 127),
                        workPlace = c.String(),
                        middleName = c.String(),
                        lastName = c.String(),
                        post = c.String(),
                        initials = c.String(),
                        firstName = c.String(),
                        division = c.String(),
                    })
                .PrimaryKey(t => t.pkey);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Teachers");
        }
    }
}
