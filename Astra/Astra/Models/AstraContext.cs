using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Management;
using System.IO;
using System.Text;
using System.Data.Entity.Validation;
using MTUtil.IO;
using MTUtil.Strings;
using MTUtil.DateTimes;
using MTUtil.TypeManagement;
using Astra.Helper;
using Astra.Models;
using Astra.Models.ResourceTypes;
using Astra.Logging;
using Astra.Migrations;
using Astra.AstraConfigurations.Settings;

namespace Astra.DatabaseContext
{
  public class AstraContext : DbContext, IAstraContext
  {
    // You can add custom code to this file. Changes will not be overwritten.
    // 
    // If you want Entity Framework to drop and regenerate your database
    // automatically whenever you change your model schema, add the following
    // code to the Application_Start method in your Global.asax file.
    // Note: this will destroy and re-create your database with every model change.
    // 
    // LINE: System.Data.Entity.Database.SetInitializer(new System.Data.Entity.DropCreateDatabaseIfModelChanges<Astra.Models.AstraContext>())

    #region DBSETS
    public DbSet<Astra.Models.AuditRecord> AuditRecords { get; set; }
    public DbSet<Astra.Models.Resource> Resources { get; set; }
    public DbSet<Astra.Models.ResourceType> ResourceTypes { get; set; }
    public DbSet<KeyWord> KeyWords { get; set; }
    public DbSet<Astra.Models.Comment> Comments { get; set; }
    public DbSet<Astra.Models.Rating> Ratings { get; set; }
    public DbSet<ResourceToKeyWordLink> ResourceToKeyWordLinks { get; set; }
    public DbSet<Astra.Models.CheckOut> CheckOuts { get; set; }
    public DbSet<Astra.Models.Reservation> Reservations { get; set; }
    public DbSet<Astra.Models.UserProfile> UserProfiles { get; set; }
    public DbSet<Astra.Models.ResourceImage> Images { get; set; }
    public DbSet<Astra.Models.ResourceSuggestion> ResourceSuggestions { get; set; }
    public DbSet<Astra.Models.EventLog> EventLogs { get; set; }
    public DbSet<Astra.Models.UserMailMessage> UserMessages { get; set; }
    public DbSet<Astra.Models.Report> Reports { get; set; }
    #endregion

    #region LIFECYCLE_CODE

    public AstraContext()
    {
      System.Data.Entity.Database.SetInitializer(new AstraContextDbInitializer());
    }

    public override int SaveChanges()
    {
      int retValue = 0;

      AstraLogger.LogDebug("Saving Changes...");

      foreach (DbEntityEntry entry in ChangeTracker.Entries())
      {
        AstraLogger.LogDebug("Saving Changes to Entity of Type '" + entry.Entity.GetType().Name + "'");

        AstraBaseModel m = entry.Entity as AstraBaseModel;
        if (m != null && (entry.State == System.Data.EntityState.Modified || entry.State == System.Data.EntityState.Added))
        {
          m.OnBeforeSave();
        }
      }

      try
      {
        retValue = base.SaveChanges();
      }
      catch (DbEntityValidationException databBaseException)
      {
        AstraLogger.LogError(databBaseException);

        // one (or more) of the entities has invalid data, so we need to write all erroring properties to
        //  to the log so we can figure out which one crashed.        
        foreach (var validationErrors in databBaseException.EntityValidationErrors)
        {
          foreach (var validationError in validationErrors.ValidationErrors)
          {
            AstraLogger.LogError("Property: " + validationError.PropertyName + ": " + validationError.ErrorMessage);
          }
        }
        throw databBaseException;
      }
      

      return retValue;
    }

    private bool ChangesPendingOnEntity(DbEntityEntry entry)
    {
      return entry.State == System.Data.EntityState.Added || entry.State == System.Data.EntityState.Modified;
    }

    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    {
      // TODO:  Add any special tables, Foreign Keys, etc...
    }

    public bool PurgeOldLogs()
    {
      // create the base DELETE statement
      string sqlStmt = @"DELETE FROM [EventLog] WHERE [Level] = {0} 
        AND [LogDate] < {1}";

      try
      {
        MaintenanceSettings settings = (MaintenanceSettings)System.Configuration.ConfigurationManager.GetSection("maintenanceSettings");

        // purge debug logs older than one month      
        this.Database.ExecuteSqlCommand(sqlStmt, "DEBUG", DateTimeUtils.ToSQLDateString(DateTime.Now.AddDays(-settings.PurgeDebugLogDays)));

        // purge info logs older than three months      
        this.Database.ExecuteSqlCommand(sqlStmt, "INFO", DateTimeUtils.ToSQLDateString(DateTime.Now.AddDays(-settings.PurgeInfoLogDays)));

        // purge error and higher-level logs older than six months      
        this.Database.ExecuteSqlCommand(sqlStmt, "ERROR", DateTimeUtils.ToSQLDateString(DateTime.Now.AddDays(-settings.PurgeErrorLogDays)));
        this.Database.ExecuteSqlCommand(sqlStmt, "FATAL", DateTimeUtils.ToSQLDateString(DateTime.Now.AddDays(-settings.PurgeErrorLogDays)));
      }
      catch (Exception e)
      {
        return false;
      }

      return true;

    }

    #endregion

    #region SEED_DATA

    public class AstraContextDbInitializer : MigrateDatabaseToLatestVersion<AstraContext, Astra.Migrations.Configuration>
    {
      public static void SeedAll(AstraContext context)
      {
        // Resource Types
        SeedResourceTypes(context);

        // Key Words
        SeedKeyWords(context);

        // Resources
        SeedResources(context);

        // Resource Images
        SeedBookThumbnails(context);

        //User Profiles
        SeedUsers(context);

        SeedReports(context);
      }

      private static void SeedUsers(AstraContext context)
      {

      }

      public static void SeedResourceTypes(AstraContext context)
      {
        context.ResourceTypes.Add(new ResourceType { Name = "Book", ResourceTypeID = 1, Icon = "book.png" });
        context.ResourceTypes.Add(new ResourceType { Name = "DVD", ResourceTypeID = 2, Icon = "application.png" });
        context.ResourceTypes.Add(new ResourceType { Name = "EBook", ResourceTypeID = 3, Icon = "wrench_orange.png" });
        context.ResourceTypes.Add(new ResourceType { Name = "Hardware", ResourceTypeID = 4, Icon = "page_white_office.png" });
        context.ResourceTypes.Add(new ResourceType { Name = "Software", ResourceTypeID = 5, Icon = "page_white_office.png" });
        context.ResourceTypes.Add(new ResourceType { Name = "White Paper", ResourceTypeID = 6, Icon = "page_white_office.png" });
      }

      public static void SeedKeyWords(AstraContext context)
      {
        context.KeyWords.Add(new KeyWord { KeyWordID = 1, Word = ".NET" });
        context.KeyWords.Add(new KeyWord { KeyWordID = 2, Word = "MVC" });
        context.KeyWords.Add(new KeyWord { KeyWordID = 3, Word = "C#" });
        context.KeyWords.Add(new KeyWord { KeyWordID = 4, Word = "Javascript" });
        context.KeyWords.Add(new KeyWord { KeyWordID = 5, Word = "JQuery" });
        context.KeyWords.Add(new KeyWord { KeyWordID = 6, Word = "(General Development)" });
        context.KeyWords.Add(new KeyWord { KeyWordID = 7, Word = "HTML" });
        context.KeyWords.Add(new KeyWord { KeyWordID = 8, Word = "HTML4" });
        context.KeyWords.Add(new KeyWord { KeyWordID = 9, Word = "HTML5" });
        context.KeyWords.Add(new KeyWord { KeyWordID = 10, Word = "XHTML" });
        context.KeyWords.Add(new KeyWord { KeyWordID = 11, Word = "CSS" });
        context.KeyWords.Add(new KeyWord { KeyWordID = 12, Word = "WPF" });
        context.KeyWords.Add(new KeyWord { KeyWordID = 13, Word = "Silverlight" });
        context.KeyWords.Add(new KeyWord { KeyWordID = 14, Word = "LINQ" });
        context.KeyWords.Add(new KeyWord { KeyWordID = 15, Word = "Entity Framework" });
        context.KeyWords.Add(new KeyWord { KeyWordID = 16, Word = "Testing" });
        context.KeyWords.Add(new KeyWord { KeyWordID = 17, Word = "XML" });
        context.KeyWords.Add(new KeyWord { KeyWordID = 18, Word = "XSL" });
        context.KeyWords.Add(new KeyWord { KeyWordID = 19, Word = "WinForms" });
        context.KeyWords.Add(new KeyWord { KeyWordID = 20, Word = "MS SQL Server" });
        context.KeyWords.Add(new KeyWord { KeyWordID = 21, Word = "Security" });
        context.KeyWords.Add(new KeyWord { KeyWordID = 22, Word = "Agile" });
        context.KeyWords.Add(new KeyWord { KeyWordID = 23, Word = "Scrum" });
        context.KeyWords.Add(new KeyWord { KeyWordID = 24, Word = "Android" });
        context.KeyWords.Add(new KeyWord { KeyWordID = 25, Word = "Mobile" });
        context.KeyWords.Add(new KeyWord { KeyWordID = 26, Word = "Apache" });
        context.KeyWords.Add(new KeyWord { KeyWordID = 27, Word = "CLR" });
        context.KeyWords.Add(new KeyWord { KeyWordID = 28, Word = "Clean Code" });
        context.KeyWords.Add(new KeyWord { KeyWordID = 29, Word = "Database" });
        context.KeyWords.Add(new KeyWord { KeyWordID = 30, Word = "SQL (General)" });
        context.KeyWords.Add(new KeyWord { KeyWordID = 31, Word = "DBA" });
        context.KeyWords.Add(new KeyWord { KeyWordID = 32, Word = "Java" });
        context.KeyWords.Add(new KeyWord { KeyWordID = 33, Word = "Google" });
        context.KeyWords.Add(new KeyWord { KeyWordID = 34, Word = "Analytics" });
        context.KeyWords.Add(new KeyWord { KeyWordID = 35, Word = "Business Intelligence" });
        context.KeyWords.Add(new KeyWord { KeyWordID = 36, Word = "BI" });
        context.KeyWords.Add(new KeyWord { KeyWordID = 37, Word = "SEO" });
        context.KeyWords.Add(new KeyWord { KeyWordID = 38, Word = "Search Engine" });
        context.KeyWords.Add(new KeyWord { KeyWordID = 39, Word = "Hadoop" });
        context.KeyWords.Add(new KeyWord { KeyWordID = 40, Word = "Hibernate" });
        context.KeyWords.Add(new KeyWord { KeyWordID = 41, Word = "ORM" });
        context.KeyWords.Add(new KeyWord { KeyWordID = 42, Word = "Exams" });
        context.KeyWords.Add(new KeyWord { KeyWordID = 43, Word = "Jenkins" });
        context.KeyWords.Add(new KeyWord { KeyWordID = 44, Word = "JIRA" });
        context.KeyWords.Add(new KeyWord { KeyWordID = 45, Word = "MCTS" });
        context.KeyWords.Add(new KeyWord { KeyWordID = 46, Word = "OOP" });
        context.KeyWords.Add(new KeyWord { KeyWordID = 47, Word = "Spring" });
        context.KeyWords.Add(new KeyWord { KeyWordID = 48, Word = "Data Warehouse" });
        context.KeyWords.Add(new KeyWord { KeyWordID = 49, Word = "Dependency Injection" });
        context.KeyWords.Add(new KeyWord { KeyWordID = 50, Word = "JQueryUI" });
        context.KeyWords.Add(new KeyWord { KeyWordID = 51, Word = "MVC3" });
        context.KeyWords.Add(new KeyWord { KeyWordID = 52, Word = "MVC4" });
        context.KeyWords.Add(new KeyWord { KeyWordID = 53, Word = "EF" });
        context.KeyWords.Add(new KeyWord { KeyWordID = 54, Word = "ADO.NET" });
        context.KeyWords.Add(new KeyWord { KeyWordID = 55, Word = "WCF" });
        context.KeyWords.Add(new KeyWord { KeyWordID = 56, Word = "WPF" });
        context.KeyWords.Add(new KeyWord { KeyWordID = 57, Word = "Silverlight" });
        context.KeyWords.Add(new KeyWord { KeyWordID = 58, Word = "MVVM" });
        context.KeyWords.Add(new KeyWord { KeyWordID = 59, Word = "Software" });
        context.KeyWords.Add(new KeyWord { KeyWordID = 60, Word = "Media" });
        context.KeyWords.Add(new KeyWord { KeyWordID = 61, Word = "Educational" });
        context.KeyWords.Add(new KeyWord { KeyWordID = 62, Word = "White Paper" });
        context.KeyWords.Add(new KeyWord { KeyWordID = 63, Word = "Hardware" });
        context.KeyWords.Add(new KeyWord { KeyWordID = 64, Word = "Cables" });
        context.KeyWords.Add(new KeyWord { KeyWordID = 65, Word = "EBook" });
      }

      public static void SeedResources(AstraContext context)
      {
        //books
        context.Resources.Add(new Book { ResourceID = 1, Title = "Clean Code: A Handbook of Agile Software Craftsmanship", Description = GetDescription(), ResourceTypeID = 1, Author = "Robert C. Martin", ISBN13 = "978-0-13-235088-4", ISBN10 = "0132350882", Pages = 300, Committed = true, CreatedOn = DateTime.UtcNow });
        context.Resources.Add(new Book { ResourceID = 2, Title = "Dependency Injection in .NET", Description = GetDescription(), Author = "Mark Seemann", ResourceTypeID = 1, ISBN13 = "978-1935182504", Pages = 300, Committed = true, CreatedOn = DateTime.UtcNow });
        context.Resources.Add(new Book { ResourceID = 3, Title = "HTML 4 for the World Wide Web", Description = GetDescription(), Edition = "4", ResourceTypeID = 1, Author = "Mark Seemann", ISBN13 = "978-0201354935", Pages = 300, Committed = true, CreatedOn = DateTime.UtcNow });
        context.Resources.Add(new Book { ResourceID = 4, Title = "Agile Product Management With Scrum: Creating Products That Customers Love", Description = GetDescription(), ResourceTypeID = 1, Author = "Unknown", Pages = 300, Committed = true, CreatedOn = DateTime.UtcNow });
        context.Resources.Add(new Book { ResourceID = 5, Title = "Android 4: Application Development", Description = GetDescription(), ResourceTypeID = 1, Author = "Unknown", Pages = 300, Committed = true, CreatedOn = DateTime.UtcNow });
        context.Resources.Add(new Book { ResourceID = 6, Title = "Apache Cookbook, 2nd Edition", Description = GetDescription(), ResourceTypeID = 1, Author = "Unknown", Pages = 300, Edition = "2nd", Committed = true, CreatedOn = DateTime.UtcNow });
        context.Resources.Add(new Book { ResourceID = 7, Title = "Automated Software Testing", Description = GetDescription(), ResourceTypeID = 1, Author = "Unknown", Pages = 300, Committed = true, CreatedOn = DateTime.UtcNow });
        context.Resources.Add(new Book { ResourceID = 8, Title = "CLR via C#", Description = GetDescription(), ResourceTypeID = 1, Author = "Unknown", Pages = 300, Committed = true, CreatedOn = DateTime.UtcNow });
        context.Resources.Add(new Book { ResourceID = 9, Title = "Code Complete", Description = GetDescription(), ResourceTypeID = 1, Author = "Unknown", Pages = 300, Committed = true, CreatedOn = DateTime.UtcNow });
        context.Resources.Add(new Book { ResourceID = 10, Title = "Database Design", Description = GetDescription(), ResourceTypeID = 1, Author = "Unknown", Pages = 300, Committed = true, CreatedOn = DateTime.UtcNow });
        context.Resources.Add(new Book { ResourceID = 11, Title = "Effective Java", Description = GetDescription(), ResourceTypeID = 1, Author = "Unknown", Pages = 300, Committed = true, CreatedOn = DateTime.UtcNow });
        context.Resources.Add(new Book { ResourceID = 12, Title = "Google Analytics", Description = GetDescription(), ResourceTypeID = 1, Author = "Unknown", Pages = 300, Committed = true, CreatedOn = DateTime.UtcNow });
        context.Resources.Add(new Book { ResourceID = 13, Title = "Hadoop: The Definitive Guide, 3rd Edition, by Tom White", Description = GetDescription(), ResourceTypeID = 1, Author = "Unknown", Pages = 300, Committed = true, CreatedOn = DateTime.UtcNow });
        context.Resources.Add(new Book { ResourceID = 14, Title = "Head First Java", Description = GetDescription(), ResourceTypeID = 1, Author = "Unknown", Pages = 300, Committed = true, CreatedOn = DateTime.UtcNow });
        context.Resources.Add(new Book { ResourceID = 15, Title = "Hibernate in Action", Description = GetDescription(), ResourceTypeID = 1, Author = "Unknown", Pages = 300, Committed = true, CreatedOn = DateTime.UtcNow });
        context.Resources.Add(new Book { ResourceID = 16, Title = "Hibernate Recipes: A Problem-Solution Approach", Description = GetDescription(), ResourceTypeID = 1, Author = "Unknown", Pages = 300, Committed = true, CreatedOn = DateTime.UtcNow });
        context.Resources.Add(new Book { ResourceID = 17, Title = "JAVA Practice Exams: OCP Java SE 6 Programmer Practice Exams", Description = GetDescription(), ResourceTypeID = 1, Author = "Unknown", Pages = 300, Committed = true, CreatedOn = DateTime.UtcNow });
        context.Resources.Add(new Book { ResourceID = 18, Title = "JavaScript & jQuery: The Missing Manual, 2nd Edition", Description = GetDescription(), ResourceTypeID = 1, Author = "Unknown", Pages = 300, Committed = true, CreatedOn = DateTime.UtcNow });
        context.Resources.Add(new Book { ResourceID = 19, Title = "JavaScript: The Definitive Guide, 6th Edition", Description = GetDescription(), ResourceTypeID = 1, Author = "Unknown", Pages = 300, Committed = true, CreatedOn = DateTime.UtcNow });
        context.Resources.Add(new Book { ResourceID = 20, Title = "Jenkins: The Definitive Guide", Description = GetDescription(), ResourceTypeID = 1, Author = "Unknown", Pages = 300, Committed = true, CreatedOn = DateTime.UtcNow });
        context.Resources.Add(new Book { ResourceID = 21, Title = "JIRA 4 Essentials (2)", Description = GetDescription(), ResourceTypeID = 1, Author = "Unknown", Pages = 300, Edition = "2nd", Committed = true, CreatedOn = DateTime.UtcNow });
        context.Resources.Add(new Book { ResourceID = 22, Title = "Lessons Learned in Software Testing", Description = GetDescription(), ResourceTypeID = 1, Author = "Unknown", Pages = 300, Committed = true, CreatedOn = DateTime.UtcNow });
        context.Resources.Add(new Book { ResourceID = 23, Title = "MCTS-Self Paced Training Kit ( 5)", Description = GetDescription(), ResourceTypeID = 1, Author = "Unknown", Pages = 300, Committed = true, CreatedOn = DateTime.UtcNow });
        context.Resources.Add(new Book { ResourceID = 24, Title = "Object Oriented Design", Description = GetDescription(), ResourceTypeID = 1, Author = "Unknown", Pages = 300, Committed = true, CreatedOn = DateTime.UtcNow });
        context.Resources.Add(new Book { ResourceID = 25, Title = "Professional Android 4 Application Development", Description = GetDescription(), ResourceTypeID = 1, Author = "Unknown", Pages = 300, Committed = true, CreatedOn = DateTime.UtcNow });
        context.Resources.Add(new Book { ResourceID = 26, Title = "SCJP: Sun Certified Programmer for Java 6", Description = GetDescription(), ResourceTypeID = 1, Author = "Unknown", Pages = 300, Committed = true, CreatedOn = DateTime.UtcNow });
        context.Resources.Add(new Book { ResourceID = 27, Title = "Software Estimation: Demystifying the Black Art", Description = GetDescription(), ResourceTypeID = 1, Author = "Unknown", Pages = 300, Committed = true, CreatedOn = DateTime.UtcNow });
        context.Resources.Add(new Book { ResourceID = 28, Title = "Spring in Action", Description = GetDescription(), ResourceTypeID = 1, Author = "Unknown", Pages = 300, Committed = true, CreatedOn = DateTime.UtcNow });
        context.Resources.Add(new Book { ResourceID = 29, Title = "Spring Recipes: A Problem-Solution Approach", Description = GetDescription(), ResourceTypeID = 1, Author = "Unknown", Pages = 300, Committed = true, CreatedOn = DateTime.UtcNow });
        context.Resources.Add(new Book { ResourceID = 30, Title = "Succeeding with Agile: Software Development Using Scrum", Description = GetDescription(), ResourceTypeID = 1, Author = "Unknown", Pages = 300, Committed = true, CreatedOn = DateTime.UtcNow });
        context.Resources.Add(new Book { ResourceID = 31, Title = "Team Geek", Description = GetDescription(), ResourceTypeID = 1, Author = "Unknown", Pages = 300, Committed = true, CreatedOn = DateTime.UtcNow });
        context.Resources.Add(new Book { ResourceID = 32, Title = "The Data Warehouse Toolkit (9 copies)", Description = GetDescription(), ResourceTypeID = 1, Author = "Unknown", Pages = 300, Committed = true, CreatedOn = DateTime.UtcNow });
        context.Resources.Add(new Book { ResourceID = 33, Title = "User Stories Applied: For Agile Software Development", Description = GetDescription(), ResourceTypeID = 1, Author = "Unknown", Pages = 300, Committed = true, CreatedOn = DateTime.UtcNow });
        context.Resources.Add(new Book { ResourceID = 34, Title = "Dependency Injection in .NET", Description = GetDescription(), ResourceTypeID = 1, Author = "Unknown", Pages = 300, Committed = true, CreatedOn = DateTime.UtcNow });
        context.Resources.Add(new Book { ResourceID = 35, Title = "HTML, XHTML, and CSS, Sixth Edition", Description = GetDescription(), ResourceTypeID = 1, Author = "Unknown", Pages = 300, Edition = "6", Committed = true, CreatedOn = DateTime.UtcNow });
        context.Resources.Add(new Book { ResourceID = 36, Title = "jQuery in Action, Second Edition", Description = GetDescription(), ResourceTypeID = 1, Author = "Unknown", Pages = 300, Edition = "2nd", Committed = true, CreatedOn = DateTime.UtcNow });
        context.Resources.Add(new Book { ResourceID = 37, Title = "jQuery UI", Description = GetDescription(), ResourceTypeID = 1, Author = "Unknown", Pages = 300, Committed = true, CreatedOn = DateTime.UtcNow });
        context.Resources.Add(new Book { ResourceID = 38, Title = "Pro LINQ Language Integrated Query in C# 2008", Description = GetDescription(), ResourceTypeID = 1, Author = "Unknown", Pages = 300, Committed = true, CreatedOn = DateTime.UtcNow });
        context.Resources.Add(new Book { ResourceID = 39, Title = "Professional ASP.NET MVC 3", Description = GetDescription(), ResourceTypeID = 1, Author = "Unknown", Pages = 300, Committed = true, CreatedOn = DateTime.UtcNow });
        context.Resources.Add(new Book { ResourceID = 40, Title = "Professional ASP.NET MVC 4", Description = GetDescription(), ResourceTypeID = 1, Author = "Unknown", Pages = 300, Committed = true, CreatedOn = DateTime.UtcNow });
        context.Resources.Add(new Book { ResourceID = 41, Title = "Programming Entity Framework: Building Data Centric Apps with the ADO.NET Entity Framework, Second Edition", Description = GetDescription(), ResourceTypeID = 1, Author = "Unknown", Pages = 300, Committed = true, CreatedOn = DateTime.UtcNow });
        context.Resources.Add(new Book { ResourceID = 42, Title = "The Art of Unit Testing: With Examples in .Net", Description = GetDescription(), ResourceTypeID = 1, Author = "Unknown", Pages = 300, Committed = true, CreatedOn = DateTime.UtcNow });
        context.Resources.Add(new Book { ResourceID = 43, Title = "The Definitive Guide to HTML 5", Description = GetDescription(), ResourceTypeID = 1, Author = "Unknown", Pages = 300, Committed = true, CreatedOn = DateTime.UtcNow });
        context.Resources.Add(new Book { ResourceID = 44, Title = "WCF 4.0 Multi-tier Services Development with LINQ to Entities", Description = GetDescription(), ResourceTypeID = 1, Author = "Unknown", Pages = 300, Committed = true, CreatedOn = DateTime.UtcNow });
        context.Resources.Add(new Book { ResourceID = 45, Title = "XML Bible", Description = GetDescription(), ResourceTypeID = 1, Author = "Unknown", Pages = 300, Committed = true, CreatedOn = DateTime.UtcNow });
        context.Resources.Add(new Book { ResourceID = 46, Title = "XSLT Cookbook", Description = GetDescription(), ResourceTypeID = 1, Author = "Unknown", Pages = 300, Committed = true, CreatedOn = DateTime.UtcNow });
        context.Resources.Add(new Book { ResourceID = 47, Title = "MVVM Survival Guide for Enterprise Architectures in Silverlight and WPF", Description = GetDescription(), ResourceTypeID = 1, Author = "Unknown", Pages = 300, Committed = true, CreatedOn = DateTime.UtcNow });

        //Hardware
        context.Resources.Add(new Hardware { ResourceID = 48, Title = "HDMI Cable", Dimensions = "5 ft long", Copies = 10, Committed = true, Description = "Cable for connecting the laptops to the TV." });
        context.Resources.Add(new Hardware { ResourceID = 49, Title = "Projector", Dimensions = "12 x 12 x 13", Copies = 10, Committed = true, Description = "Device for projecting laptop display." });
        //Media
        context.Resources.Add(new DVD { ResourceID = 50, Title = "Ted Talk", Runtime = "25 minutes", Copies = 1, Committed = true, Description = "A great TED Talk about technological advancements." });
        //Software
        context.Resources.Add(new Software
        {
          ResourceID = 51,
          Title = "SQL Server 2008",
          Description = @"
Offers innovative features for virtualization, power savings, and manageability. Features powerful tools to give you greater control over your servers and streamline configuration and management tasks. Builds on the award-winning foundation of Windows Server 2008, expanding existing technology and adding new features to enable IT professionals to increase the reliability and flexibility of their server infrastructures. Powerful tools such as Internet Information Services (IIS) version 7.5, updated Server Manager and Hyper-V platforms, and Windows PowerShell version 2.0 combine to give customers greater control, increased efficiency, and the ability to react to front-line business needs faster than ever before. Provides more cost-effective and reliable support for mission-critical workloads. Makes it easier for mobile workers to access company resources. Enhanced security features work to harden the operating system to help protect your data and network, while providing a solid, highly dependable foundation for your business. New virtualization tools, web resources, management enhancements, and exciting Windows 7 integration help save time, reduce costs, and provide a platform for a dynamic and efficiently managed data center."
        });
        
        context.Resources.Add(new EBook { ResourceID = 52, Title = "Sobroto's EBook of Success", Url = "http://www.someurl.com", Description = "Chairman of Mindtree writes a new book on success.", Author = "Sobroto Bagchi" });



        // also add the keyword links
        context.ResourceToKeyWordLinks.Add(new ResourceToKeyWordLink { KeyWordID = 2, ResourceID = 2 });
        context.ResourceToKeyWordLinks.Add(new ResourceToKeyWordLink { KeyWordID = 8, ResourceID = 3 });
        context.ResourceToKeyWordLinks.Add(new ResourceToKeyWordLink { KeyWordID = 22, ResourceID = 4 });
        context.ResourceToKeyWordLinks.Add(new ResourceToKeyWordLink { KeyWordID = 23, ResourceID = 4 });
        context.ResourceToKeyWordLinks.Add(new ResourceToKeyWordLink { KeyWordID = 24, ResourceID = 5 });
        context.ResourceToKeyWordLinks.Add(new ResourceToKeyWordLink { KeyWordID = 25, ResourceID = 5 });
        context.ResourceToKeyWordLinks.Add(new ResourceToKeyWordLink { KeyWordID = 26, ResourceID = 6 });
        context.ResourceToKeyWordLinks.Add(new ResourceToKeyWordLink { KeyWordID = 16, ResourceID = 7 });
        context.ResourceToKeyWordLinks.Add(new ResourceToKeyWordLink { KeyWordID = 27, ResourceID = 8 });
        context.ResourceToKeyWordLinks.Add(new ResourceToKeyWordLink { KeyWordID = 3, ResourceID = 8 });
        context.ResourceToKeyWordLinks.Add(new ResourceToKeyWordLink { KeyWordID = 28, ResourceID = 9 });
        context.ResourceToKeyWordLinks.Add(new ResourceToKeyWordLink { KeyWordID = 29, ResourceID = 10 });
        context.ResourceToKeyWordLinks.Add(new ResourceToKeyWordLink { KeyWordID = 31, ResourceID = 10 });
        context.ResourceToKeyWordLinks.Add(new ResourceToKeyWordLink { KeyWordID = 32, ResourceID = 11 });
        context.ResourceToKeyWordLinks.Add(new ResourceToKeyWordLink { KeyWordID = 33, ResourceID = 12 });
        context.ResourceToKeyWordLinks.Add(new ResourceToKeyWordLink { KeyWordID = 34, ResourceID = 12 });
        context.ResourceToKeyWordLinks.Add(new ResourceToKeyWordLink { KeyWordID = 35, ResourceID = 12 });
        context.ResourceToKeyWordLinks.Add(new ResourceToKeyWordLink { KeyWordID = 36, ResourceID = 12 });
        context.ResourceToKeyWordLinks.Add(new ResourceToKeyWordLink { KeyWordID = 37, ResourceID = 12 });
        context.ResourceToKeyWordLinks.Add(new ResourceToKeyWordLink { KeyWordID = 38, ResourceID = 12 });
        context.ResourceToKeyWordLinks.Add(new ResourceToKeyWordLink { KeyWordID = 39, ResourceID = 13 });
        context.ResourceToKeyWordLinks.Add(new ResourceToKeyWordLink { KeyWordID = 32, ResourceID = 14 });
        context.ResourceToKeyWordLinks.Add(new ResourceToKeyWordLink { KeyWordID = 40, ResourceID = 15 });
        context.ResourceToKeyWordLinks.Add(new ResourceToKeyWordLink { KeyWordID = 41, ResourceID = 15 });
        context.ResourceToKeyWordLinks.Add(new ResourceToKeyWordLink { KeyWordID = 40, ResourceID = 16 });
        context.ResourceToKeyWordLinks.Add(new ResourceToKeyWordLink { KeyWordID = 41, ResourceID = 16 });
        context.ResourceToKeyWordLinks.Add(new ResourceToKeyWordLink { KeyWordID = 42, ResourceID = 17 });
        context.ResourceToKeyWordLinks.Add(new ResourceToKeyWordLink { KeyWordID = 32, ResourceID = 17 });
        context.ResourceToKeyWordLinks.Add(new ResourceToKeyWordLink { KeyWordID = 5, ResourceID = 18 });
        context.ResourceToKeyWordLinks.Add(new ResourceToKeyWordLink { KeyWordID = 4, ResourceID = 18 });
        context.ResourceToKeyWordLinks.Add(new ResourceToKeyWordLink { KeyWordID = 4, ResourceID = 19 });
        context.ResourceToKeyWordLinks.Add(new ResourceToKeyWordLink { KeyWordID = 43, ResourceID = 20 });
        context.ResourceToKeyWordLinks.Add(new ResourceToKeyWordLink { KeyWordID = 44, ResourceID = 21 });
        context.ResourceToKeyWordLinks.Add(new ResourceToKeyWordLink { KeyWordID = 16, ResourceID = 22 });
        context.ResourceToKeyWordLinks.Add(new ResourceToKeyWordLink { KeyWordID = 42, ResourceID = 23 });
        context.ResourceToKeyWordLinks.Add(new ResourceToKeyWordLink { KeyWordID = 45, ResourceID = 23 });
        context.ResourceToKeyWordLinks.Add(new ResourceToKeyWordLink { KeyWordID = 46, ResourceID = 24 });
        context.ResourceToKeyWordLinks.Add(new ResourceToKeyWordLink { KeyWordID = 24, ResourceID = 25 });
        context.ResourceToKeyWordLinks.Add(new ResourceToKeyWordLink { KeyWordID = 25, ResourceID = 25 });
        context.ResourceToKeyWordLinks.Add(new ResourceToKeyWordLink { KeyWordID = 32, ResourceID = 26 });
        context.ResourceToKeyWordLinks.Add(new ResourceToKeyWordLink { KeyWordID = 42, ResourceID = 26 });
        context.ResourceToKeyWordLinks.Add(new ResourceToKeyWordLink { KeyWordID = 47, ResourceID = 28 });
        context.ResourceToKeyWordLinks.Add(new ResourceToKeyWordLink { KeyWordID = 47, ResourceID = 29 });
        context.ResourceToKeyWordLinks.Add(new ResourceToKeyWordLink { KeyWordID = 22, ResourceID = 30 });
        context.ResourceToKeyWordLinks.Add(new ResourceToKeyWordLink { KeyWordID = 23, ResourceID = 30 });
        context.ResourceToKeyWordLinks.Add(new ResourceToKeyWordLink { KeyWordID = 36, ResourceID = 32 });
        context.ResourceToKeyWordLinks.Add(new ResourceToKeyWordLink { KeyWordID = 48, ResourceID = 32 });
        context.ResourceToKeyWordLinks.Add(new ResourceToKeyWordLink { KeyWordID = 22, ResourceID = 33 });
        context.ResourceToKeyWordLinks.Add(new ResourceToKeyWordLink { KeyWordID = 1, ResourceID = 34 });
        context.ResourceToKeyWordLinks.Add(new ResourceToKeyWordLink { KeyWordID = 49, ResourceID = 34 });
        context.ResourceToKeyWordLinks.Add(new ResourceToKeyWordLink { KeyWordID = 7, ResourceID = 35 });
        context.ResourceToKeyWordLinks.Add(new ResourceToKeyWordLink { KeyWordID = 8, ResourceID = 35 });
        context.ResourceToKeyWordLinks.Add(new ResourceToKeyWordLink { KeyWordID = 11, ResourceID = 35 });
        context.ResourceToKeyWordLinks.Add(new ResourceToKeyWordLink { KeyWordID = 10, ResourceID = 35 });
        context.ResourceToKeyWordLinks.Add(new ResourceToKeyWordLink { KeyWordID = 5, ResourceID = 36 });
        context.ResourceToKeyWordLinks.Add(new ResourceToKeyWordLink { KeyWordID = 5, ResourceID = 37 });
        context.ResourceToKeyWordLinks.Add(new ResourceToKeyWordLink { KeyWordID = 50, ResourceID = 37 });
        context.ResourceToKeyWordLinks.Add(new ResourceToKeyWordLink { KeyWordID = 14, ResourceID = 38 });
        context.ResourceToKeyWordLinks.Add(new ResourceToKeyWordLink { KeyWordID = 3, ResourceID = 38 });
        context.ResourceToKeyWordLinks.Add(new ResourceToKeyWordLink { KeyWordID = 2, ResourceID = 39 });
        context.ResourceToKeyWordLinks.Add(new ResourceToKeyWordLink { KeyWordID = 51, ResourceID = 39 });
        context.ResourceToKeyWordLinks.Add(new ResourceToKeyWordLink { KeyWordID = 2, ResourceID = 40 });
        context.ResourceToKeyWordLinks.Add(new ResourceToKeyWordLink { KeyWordID = 52, ResourceID = 40 });
        context.ResourceToKeyWordLinks.Add(new ResourceToKeyWordLink { KeyWordID = 15, ResourceID = 41 });
        context.ResourceToKeyWordLinks.Add(new ResourceToKeyWordLink { KeyWordID = 53, ResourceID = 41 });
        context.ResourceToKeyWordLinks.Add(new ResourceToKeyWordLink { KeyWordID = 54, ResourceID = 41 });
        context.ResourceToKeyWordLinks.Add(new ResourceToKeyWordLink { KeyWordID = 16, ResourceID = 42 });
        context.ResourceToKeyWordLinks.Add(new ResourceToKeyWordLink { KeyWordID = 1, ResourceID = 42 });
        context.ResourceToKeyWordLinks.Add(new ResourceToKeyWordLink { KeyWordID = 7, ResourceID = 43 });
        context.ResourceToKeyWordLinks.Add(new ResourceToKeyWordLink { KeyWordID = 9, ResourceID = 43 });
        context.ResourceToKeyWordLinks.Add(new ResourceToKeyWordLink { KeyWordID = 55, ResourceID = 44 });
        context.ResourceToKeyWordLinks.Add(new ResourceToKeyWordLink { KeyWordID = 14, ResourceID = 44 });
        context.ResourceToKeyWordLinks.Add(new ResourceToKeyWordLink { KeyWordID = 17, ResourceID = 45 });
        context.ResourceToKeyWordLinks.Add(new ResourceToKeyWordLink { KeyWordID = 17, ResourceID = 46 });
        context.ResourceToKeyWordLinks.Add(new ResourceToKeyWordLink { KeyWordID = 18, ResourceID = 46 });
        context.ResourceToKeyWordLinks.Add(new ResourceToKeyWordLink { KeyWordID = 56, ResourceID = 47 });
        context.ResourceToKeyWordLinks.Add(new ResourceToKeyWordLink { KeyWordID = 57, ResourceID = 47 });
        context.ResourceToKeyWordLinks.Add(new ResourceToKeyWordLink { KeyWordID = 58, ResourceID = 47 });
        context.ResourceToKeyWordLinks.Add(new ResourceToKeyWordLink { KeyWordID = 63, ResourceID = 48 });
        context.ResourceToKeyWordLinks.Add(new ResourceToKeyWordLink { KeyWordID = 64, ResourceID = 48 });
        context.ResourceToKeyWordLinks.Add(new ResourceToKeyWordLink { KeyWordID = 63, ResourceID = 49 });
        context.ResourceToKeyWordLinks.Add(new ResourceToKeyWordLink { KeyWordID = 60, ResourceID = 50 });
        context.ResourceToKeyWordLinks.Add(new ResourceToKeyWordLink { KeyWordID = 61, ResourceID = 50 });
        context.ResourceToKeyWordLinks.Add(new ResourceToKeyWordLink { KeyWordID = 59, ResourceID = 51 });
        context.ResourceToKeyWordLinks.Add(new ResourceToKeyWordLink { KeyWordID = 65, ResourceID = 52 });
        context.ResourceToKeyWordLinks.Add(new ResourceToKeyWordLink { KeyWordID = 6, ResourceID = 1 });
        context.ResourceToKeyWordLinks.Add(new ResourceToKeyWordLink { KeyWordID = 6, ResourceID = 9 });
        context.ResourceToKeyWordLinks.Add(new ResourceToKeyWordLink { KeyWordID = 6, ResourceID = 27 });
        context.ResourceToKeyWordLinks.Add(new ResourceToKeyWordLink { KeyWordID = 6, ResourceID = 31 });
      }

      private static void SeedBookThumbnails(AstraContext context)
      {
        string junkPath = @"C:\Temp\AstraBooks";

        if (!FileUtils.FolderExists(junkPath))
          return;

        string[] thumbFiles = Directory.GetFiles(junkPath, "*Resource*.jpg");
        foreach (string thumbFile in thumbFiles)
        {
          FileInfo fi = new FileInfo(thumbFile);
          string name = Path.GetFileNameWithoutExtension(fi.FullName);
          List<string> args = StringUtils.SplitToList(name, "_", StringSplitOptions.None);

          int resourceId = TypeUtils.ToInt(args[1]);
          Resource r = FindResourceCached(resourceId, context);

          if (r == null)
            continue;

          byte[] data = FileUtils.ReadFileToByteArray(fi);
          ResourceImage ri = new ResourceImage();
          ri.ImageData = data;
          ri.ImageThumbnail = data;
          ri.Caption = "[Placeholder Caption]";
          ri.ContentType = "image";

          r.CoverImage = ri;
        }

      }

      private static void SeedReports(AstraContext context)
      {
        context.Reports.Add(new Report
        {
          ReportID = 1,
          Title = "Overdue Books",
          Description = "Shows all overdue books",
          IsRemote = true,
          Context = "Resources",
          ReportPath = "/Astra Overdue Resources Report/Over Due"
        });

        context.Reports.Add(new Report
        {
          ReportID = 2,
          Title = "Books Utilization",
          Description = "Shows the Check-Out Rate for All Books",
          IsRemote = true,
          Context = "Resources",
          ReportPath = "/Astra Resource Usage Report/Astra Resource Usage Report"
        });

        context.Reports.Add(new Report
        {
          ReportID = 3,
          Title = "User Check-Outs",
          Description = "Shows Check-Out History for All Users",
          IsRemote = true,
          Context = "Resources",
          ReportPath = "/Astra UserCheckOuts Report/Astra UserCheckOuts Report"
        });

        context.Reports.Add(new Report
        {
          ReportID = 4,
          Title = "Resources Inventory",
          Description = "Inventory of Resources in Astra Database",
          IsRemote = true,
          Context = "Resources",
          ReportPath = "/InventoryReport/InventoryReport"
        });
      }

      protected static Resource FindResourceCached(int resourceId, AstraContext context)
      {
        Resource r = context.Resources.Find(resourceId);
        return r;
      }

      private static string GetDescription()
      {
        const string LATIN_WORDS = "lorem ipsum dolor sit amet consectetuer adipiscing elit sed diam nonummy nibh euismod tincidunt ut laoreet dolore magna aliquam erat volutpat ut wisi enim ad minim veniam quis nostrud exerci tation ullamcorper suscipit lobortis nisl ut aliquip ex ea commodo consequat duis autem vel eum iriure dolor in hendrerit in vulputate velit esse molestie consequat vel illum dolore eu feugiat nulla facilisis at vero eros et accumsan et iusto odio dignissim qui blandit praesent luptatum zzril delenit augue duis dolore te feugait nulla facilisi Ut wisi enim ad minim veniam quis nostrud exerci tation ullamcorper suscipit lobortis nisl ut aliquip ex ea commodo consequat duis autem vel eum iriure dolor in hendrerit in vulputate velit esse molestie consequat vel illum dolore eu feugiat nulla facilisis at vero eros et accumsan et iusto odio dignissim qui blandit praesent luptatum zzril delenit augue duis dolore te feugait nulla facilisi";
        List<string> words = StringUtils.SplitToList(LATIN_WORDS, " ", StringSplitOptions.None);

        Random r = MTUtil.Mathematic.MathUtils.Random();        
        StringBuilder sb = new StringBuilder();
                
        int numSentences = r.Next(7, 12);
        for (int i = 0; i < numSentences; i++)
        {
          if (i > 0)          
            sb.Append(". ");

          int numWords = r.Next(6, 20);
          for (int k = 0; k < numWords; k++)
          {
            if (k == 0)
              sb.Append(StringUtils.CapsFirst(words[r.Next(0, words.Count - 1)]));
            else
              sb.Append(" " + words[r.Next(0, words.Count - 1)]);
          }

        }
        sb.Append(".");
        return StringUtils.Left(sb.ToString(), 1000);
      }

    }

    #endregion

  }
}