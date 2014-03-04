namespace Astra.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ResourceTypes : DbMigration
    {
        public override void Up()
        {
          AddColumn("dbo.Resources", "Pages", c => c.Int(defaultValue: 0));
            AddColumn("dbo.Resources", "Actors", c => c.String());
            AddColumn("dbo.Resources", "Directors", c => c.String());
            AddColumn("dbo.Resources", "Writers", c => c.String());
            AddColumn("dbo.Resources", "Producers", c => c.String());
            AddColumn("dbo.Resources", "Format", c => c.String());
            AddColumn("dbo.Resources", "Language", c => c.String());
            AddColumn("dbo.Resources", "Subtitles", c => c.String());
            AddColumn("dbo.Resources", "Studio", c => c.String());
            AddColumn("dbo.Resources", "Releasedate", c => c.DateTime());
            AddColumn("dbo.Resources", "Runtime", c => c.String());
            AddColumn("dbo.Resources", "Dimensions", c => c.String());
            AddColumn("dbo.Resources", "Modelno", c => c.String());
            AddColumn("dbo.Resources", "Discriminator", c => c.String(nullable: false, maxLength: 128, defaultValue: "Book"));

            Sql("update Resources set pages = length where pages is null");
            
            DropColumn("dbo.Resources", "SkillLevel");
            DropColumn("dbo.Resources", "Length");
            DropColumn("dbo.Resources", "MediaTypeId");
            DropTable("dbo.MediaTypes");
        }
        
        public override void Down()
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
            AddColumn("dbo.Resources", "Length", c => c.Int(nullable: false));
            AddColumn("dbo.Resources", "SkillLevel", c => c.Int(nullable: false));
            DropColumn("dbo.Resources", "Discriminator");
            DropColumn("dbo.Resources", "Modelno");
            DropColumn("dbo.Resources", "Dimensions");
            DropColumn("dbo.Resources", "Runtime");
            DropColumn("dbo.Resources", "Releasedate");
            DropColumn("dbo.Resources", "Studio");
            DropColumn("dbo.Resources", "Subtitles");
            DropColumn("dbo.Resources", "Language");
            DropColumn("dbo.Resources", "Format");
            DropColumn("dbo.Resources", "Producers");
            DropColumn("dbo.Resources", "Writers");
            DropColumn("dbo.Resources", "Directors");
            DropColumn("dbo.Resources", "Actors");
            DropColumn("dbo.Resources", "Pages");
        }
    }
}
