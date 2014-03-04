namespace Astra.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedUserPreferences : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserProfile", "Preferences", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserProfile", "Preferences");
        }
    }
}
