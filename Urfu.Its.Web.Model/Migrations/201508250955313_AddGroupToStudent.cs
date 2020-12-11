namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddGroupToStudent : DbMigration
    {
        public override void Up()
        {
            Sql("delete from students where not exists(select 1 from groups g where g.Id=GroupId)");
            DropIndex("dbo.Students", "IX_Student_GroupId");
            AlterColumn("dbo.Students", "GroupId", c => c.String(maxLength: 128));
            CreateIndex("dbo.Students", "GroupId", name: "IX_Student_GroupId");
            AddForeignKey("dbo.Students", "GroupId", "dbo.Groups", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Students", "GroupId", "dbo.Groups");
            DropIndex("dbo.Students", "IX_Student_GroupId");
            AlterColumn("dbo.Students", "GroupId", c => c.String(maxLength: 127));
            CreateIndex("dbo.Students", "GroupId", name: "IX_Student_GroupId");
        }
    }
}
