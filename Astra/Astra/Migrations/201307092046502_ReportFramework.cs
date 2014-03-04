namespace Astra.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ReportFramework : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Reports",
                c => new
                    {
                        ReportID = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 255),
                        Description = c.String(maxLength: 2000),
                        IsRemote = c.Boolean(nullable: false),
                        ReportPath = c.String(maxLength: 255),
                        Context = c.String(maxLength: 255),
                    })
                .PrimaryKey(t => t.ReportID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Reports");
        }
    }
}
