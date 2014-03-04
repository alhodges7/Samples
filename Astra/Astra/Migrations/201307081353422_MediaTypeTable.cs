namespace Astra.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MediaTypeTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MediaTypes",
                c => new
                    {
                        MediaTypeID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.MediaTypeID);
            
            AddColumn("dbo.Resources", "MediaTypeId", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Resources", "MediaTypeId");
            DropTable("dbo.MediaTypes");
        }
    }
}
