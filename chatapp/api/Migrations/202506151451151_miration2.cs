namespace api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class miration2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ValidationSessionModels", "createdAt", c => c.DateTime(nullable: false));
            AddColumn("dbo.ValidationSessionModels", "expiration", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ValidationSessionModels", "expiration");
            DropColumn("dbo.ValidationSessionModels", "createdAt");
        }
    }
}
