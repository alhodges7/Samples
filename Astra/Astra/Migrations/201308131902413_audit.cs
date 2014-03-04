namespace Astra.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Audit : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AuditRecords",
                c => new
                    {
                        AuditId = c.Guid(nullable: false),
                        SessionID = c.String(),
                        IpAddress = c.String(),
                        Mid = c.String(),
                        UrlAccessed = c.String(),
                        Data = c.String(),
                        CreatedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.AuditId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.AuditRecords");
        }
    }
}
