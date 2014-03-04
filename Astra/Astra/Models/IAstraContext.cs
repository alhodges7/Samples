using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using Astra.Models;

namespace Astra.DatabaseContext
{
  public interface IAstraContext
  {
    #region DBSETS
    DbSet<Astra.Models.AuditRecord> AuditRecords { get; set; }
    DbSet<Astra.Models.Resource> Resources { get; set; }
    DbSet<Astra.Models.ResourceType> ResourceTypes { get; set; }
    DbSet<KeyWord> KeyWords { get; set; }
    DbSet<Astra.Models.Comment> Comments { get; set; }
    DbSet<Astra.Models.Rating> Ratings { get; set; }
    DbSet<ResourceToKeyWordLink> ResourceToKeyWordLinks { get; set; }
    DbSet<Astra.Models.CheckOut> CheckOuts { get; set; }
    DbSet<Astra.Models.Reservation> Reservations { get; set; }
    DbSet<Astra.Models.UserProfile> UserProfiles { get; set; }
    DbSet<Astra.Models.ResourceImage> Images { get; set; }
    DbSet<Astra.Models.ResourceSuggestion> ResourceSuggestions { get; set; }
    DbSet<Astra.Models.EventLog> EventLogs { get; set; }
    DbSet<Astra.Models.UserMailMessage> UserMessages { get; set; }
    DbSet<Astra.Models.Report> Reports { get; set; }
    #endregion

    bool PurgeOldLogs();
    int SaveChanges();
  }
}
