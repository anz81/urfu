namespace Urfu.Its.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDurationFieldsToTrainingDurationsTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TrainingDurations", "DurationSPO", c => c.Decimal(nullable: true, precision: 18, scale: 2));
            AddColumn("dbo.TrainingDurations", "DurationSPOUnsuitableProfile", c => c.Decimal(nullable: true, precision: 18, scale: 2));
            AddColumn("dbo.TrainingDurations", "DurationVPO", c => c.Decimal(nullable: true, precision: 18, scale: 2));
            AddColumn("dbo.TrainingDurations", "DurationVPOUnsuitableProfile", c => c.Decimal(nullable: true, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TrainingDurations", "DurationVPOUnsuitableProfile");
            DropColumn("dbo.TrainingDurations", "DurationVPO");
            DropColumn("dbo.TrainingDurations", "DurationSPOUnsuitableProfile");
            DropColumn("dbo.TrainingDurations", "DurationSPO");
        }
    }
}
