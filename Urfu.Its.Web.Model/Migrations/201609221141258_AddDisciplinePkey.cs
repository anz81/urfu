namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddDisciplinePkey : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Disciplines", "pkey", c => c.String(maxLength: 32));
            Sql("UPDATE dbo.Disciplines SET pkey = SUBSTRING(uid, 7, 32)");
            AlterColumn("dbo.Disciplines", "pkey", c => c.String(nullable: false));
        }

        public override void Down()
        {
            DropColumn("dbo.Disciplines", "pkey");
        }
    }
}
