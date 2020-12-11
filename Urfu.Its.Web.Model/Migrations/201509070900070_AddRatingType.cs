namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddRatingType : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Students", "RatingType", c => c.Int());
            Sql("update Students set RatingType = 1 where Rating is not null");
        }
        
        public override void Down()
        {
            DropColumn("dbo.Students", "RatingType");
        }
    }
}
