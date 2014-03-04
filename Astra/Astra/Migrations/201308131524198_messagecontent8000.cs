namespace Astra.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Messagecontent8000 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.UserMailMessages", "MessageContent", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.UserMailMessages", "MessageContent", c => c.String(maxLength: 4000));
        }
    }
}
