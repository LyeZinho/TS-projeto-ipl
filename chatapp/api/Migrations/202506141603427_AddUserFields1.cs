namespace api.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserFields1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserModels", "UniqueId", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserModels", "UniqueId");
        }
    }
}
