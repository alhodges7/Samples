namespace Astra.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Resources",
                c => new
                    {
                        ResourceID = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 255),
                        Edition = c.String(maxLength: 50),
                        ResourceTypeID = c.Int(),
                        Description = c.String(maxLength: 4000),
                        Author = c.String(maxLength: 255),
                        Publisher = c.String(maxLength: 255),
                        Url = c.String(maxLength: 255),
                        Committed = c.Boolean(nullable: false),
                        ISBN10 = c.String(maxLength: 50),
                        ISBN13 = c.String(maxLength: 50),
                        SkillLevel = c.Int(nullable: false),
                        Length = c.Int(nullable: false),
                        Copies = c.Int(nullable: false),
                        CoverImageId = c.Int(),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedByMID = c.String(),
                        LastModifiedOn = c.DateTime(nullable: false),
                        LastModifiedByMID = c.String(),
                    })
                .PrimaryKey(t => t.ResourceID)
                .ForeignKey("dbo.ResourceTypes", t => t.ResourceTypeID)
                .ForeignKey("dbo.ResourceImages", t => t.CoverImageId)
                .Index(t => t.ResourceTypeID)
                .Index(t => t.CoverImageId);
            
            CreateTable(
                "dbo.CheckOuts",
                c => new
                    {
                        CheckOutID = c.Int(nullable: false, identity: true),
                        ResourceID = c.Int(nullable: false),
                        UserMID = c.String(nullable: false),
                        CheckOutStatus = c.Int(nullable: false),
                        DateCheckedOut = c.DateTime(nullable: false),
                        DateCheckedIn = c.DateTime(nullable: false),
                        DaysAllowed = c.Int(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedByMID = c.String(),
                        LastModifiedOn = c.DateTime(nullable: false),
                        LastModifiedByMID = c.String(),
                    })
                .PrimaryKey(t => t.CheckOutID)
                .ForeignKey("dbo.Resources", t => t.ResourceID, cascadeDelete: true)
                .Index(t => t.ResourceID);
            
            CreateTable(
                "dbo.ResourceTypes",
                c => new
                    {
                        ResourceTypeID = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 255),
                        Icon = c.String(maxLength: 255),
                        Notes = c.String(),
                    })
                .PrimaryKey(t => t.ResourceTypeID);
            
            CreateTable(
                "dbo.Comments",
                c => new
                    {
                        CommentId = c.Int(nullable: false, identity: true),
                        ResourceID = c.Int(nullable: false),
                        UserMID = c.String(nullable: false),
                        UserComment = c.String(),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedByMID = c.String(),
                        LastModifiedOn = c.DateTime(nullable: false),
                        LastModifiedByMID = c.String(),
                        Rating_RatingId = c.Int(),
                    })
                .PrimaryKey(t => t.CommentId)
                .ForeignKey("dbo.Resources", t => t.ResourceID, cascadeDelete: true)
                .ForeignKey("dbo.Ratings", t => t.Rating_RatingId)
                .Index(t => t.ResourceID)
                .Index(t => t.Rating_RatingId);
            
            CreateTable(
                "dbo.Ratings",
                c => new
                    {
                        RatingId = c.Int(nullable: false, identity: true),
                        ResourceID = c.Int(nullable: false),
                        UserMID = c.String(),
                        UserRating = c.Double(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedByMID = c.String(),
                        LastModifiedOn = c.DateTime(nullable: false),
                        LastModifiedByMID = c.String(),
                    })
                .PrimaryKey(t => t.RatingId)
                .ForeignKey("dbo.Resources", t => t.ResourceID, cascadeDelete: true)
                .Index(t => t.ResourceID);
            
            CreateTable(
                "dbo.ResourceImages",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        ImageData = c.Binary(nullable: false),
                        ImageThumbnail = c.Binary(nullable: false),
                        ContentType = c.String(maxLength: 1000),
                        Caption = c.String(maxLength: 1000),
                        Resource_ResourceID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Resources", t => t.Resource_ResourceID)
                .Index(t => t.Resource_ResourceID);
            
            CreateTable(
                "dbo.KeyWords",
                c => new
                    {
                        KeyWordID = c.Int(nullable: false, identity: true),
                        Word = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.KeyWordID);
            
            CreateTable(
                "dbo.ResourceToKeyWordLinks",
                c => new
                    {
                        LinkID = c.Int(nullable: false, identity: true),
                        KeyWordID = c.Int(nullable: false),
                        ResourceID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.LinkID)
                .ForeignKey("dbo.KeyWords", t => t.KeyWordID, cascadeDelete: true)
                .ForeignKey("dbo.Resources", t => t.ResourceID, cascadeDelete: true)
                .Index(t => t.KeyWordID)
                .Index(t => t.ResourceID);
            
            CreateTable(
                "dbo.Reservations",
                c => new
                    {
                        ReservationID = c.Int(nullable: false, identity: true),
                        ReservationStatus = c.Int(nullable: false),
                        ResourceID = c.Int(nullable: false),
                        UserMID = c.String(nullable: false),
                        DateReserved = c.DateTime(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedByMID = c.String(),
                        LastModifiedOn = c.DateTime(nullable: false),
                        LastModifiedByMID = c.String(),
                    })
                .PrimaryKey(t => t.ReservationID)
                .ForeignKey("dbo.Resources", t => t.ResourceID, cascadeDelete: true)
                .Index(t => t.ResourceID);
            
            CreateTable(
                "dbo.UserProfile",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MID = c.String(),
                        LastName = c.String(nullable: false, maxLength: 75),
                        FirstName = c.String(nullable: false, maxLength: 75),
                        Email = c.String(maxLength: 255),
                        IsActive = c.Boolean(nullable: false),                        
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ResourceSuggestions",
                c => new
                    {
                        ResourceSuggestionId = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 500),
                        Description = c.String(nullable: false),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ReasonNeeded = c.String(nullable: false),
                        URL = c.String(),
                        LibrariansNote = c.String(),
                        Status = c.Int(nullable: false),
                        ISBN10 = c.String(maxLength: 50),
                        ISBN13 = c.String(maxLength: 50),
                        CreatedOn = c.DateTime(nullable: false),
                        CreatedByMID = c.String(),
                        LastModifiedOn = c.DateTime(nullable: false),
                        LastModifiedByMID = c.String(),
                    })
                .PrimaryKey(t => t.ResourceSuggestionId);
            
            CreateTable(
                "dbo.EventLog",
                c => new
                    {
                        EventLogID = c.Int(nullable: false, identity: true),
                        LogDate = c.DateTime(nullable: false),
                        Thread = c.String(maxLength: 255),
                        Level = c.String(maxLength: 20),
                        Logger = c.String(maxLength: 255),
                        Message = c.String(maxLength: 4000),
                        Exception = c.String(maxLength: 4000),
                    })
                .PrimaryKey(t => t.EventLogID);
            
            CreateTable(
                "dbo.UserMailMessages",
                c => new
                    {
                        MessageId = c.Int(nullable: false, identity: true),
                        Type = c.Int(nullable: false),
                        ReadState = c.Int(nullable: false),
                        OriginatorMid = c.String(),
                        RecipientMid = c.String(),
                        MessageContent = c.String(maxLength: 4000),
                        SentOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.MessageId);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.Reservations", new[] { "ResourceID" });
            DropIndex("dbo.ResourceToKeyWordLinks", new[] { "ResourceID" });
            DropIndex("dbo.ResourceToKeyWordLinks", new[] { "KeyWordID" });
            DropIndex("dbo.ResourceImages", new[] { "Resource_ResourceID" });
            DropIndex("dbo.Ratings", new[] { "ResourceID" });
            DropIndex("dbo.Comments", new[] { "Rating_RatingId" });
            DropIndex("dbo.Comments", new[] { "ResourceID" });
            DropIndex("dbo.CheckOuts", new[] { "ResourceID" });
            DropIndex("dbo.Resources", new[] { "CoverImageId" });
            DropIndex("dbo.Resources", new[] { "ResourceTypeID" });
            DropForeignKey("dbo.Reservations", "ResourceID", "dbo.Resources");
            DropForeignKey("dbo.ResourceToKeyWordLinks", "ResourceID", "dbo.Resources");
            DropForeignKey("dbo.ResourceToKeyWordLinks", "KeyWordID", "dbo.KeyWords");
            DropForeignKey("dbo.ResourceImages", "Resource_ResourceID", "dbo.Resources");
            DropForeignKey("dbo.Ratings", "ResourceID", "dbo.Resources");
            DropForeignKey("dbo.Comments", "Rating_RatingId", "dbo.Ratings");
            DropForeignKey("dbo.Comments", "ResourceID", "dbo.Resources");
            DropForeignKey("dbo.CheckOuts", "ResourceID", "dbo.Resources");
            DropForeignKey("dbo.Resources", "CoverImageId", "dbo.ResourceImages");
            DropForeignKey("dbo.Resources", "ResourceTypeID", "dbo.ResourceTypes");
            DropTable("dbo.UserMailMessages");
            DropTable("dbo.EventLog");
            DropTable("dbo.ResourceSuggestions");
            DropTable("dbo.UserProfile");
            DropTable("dbo.Reservations");
            DropTable("dbo.ResourceToKeyWordLinks");
            DropTable("dbo.KeyWords");
            DropTable("dbo.ResourceImages");
            DropTable("dbo.Ratings");
            DropTable("dbo.Comments");
            DropTable("dbo.ResourceTypes");
            DropTable("dbo.CheckOuts");
            DropTable("dbo.Resources");
        }
    }
}
