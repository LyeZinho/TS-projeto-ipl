namespace api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class migration1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ValidationSessionModels", "AttemptUsername", c => c.String());
            AddColumn("dbo.ValidationSessionModels", "AttemptPassword", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ValidationSessionModels", "AttemptPassword");
            DropColumn("dbo.ValidationSessionModels", "AttemptUsername");
        }
    }
}
