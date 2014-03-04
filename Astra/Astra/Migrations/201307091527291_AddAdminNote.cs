namespace Astra.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAdminNote : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Resources", "AdminNote", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Resources", "AdminNote");
        }
    }
}
