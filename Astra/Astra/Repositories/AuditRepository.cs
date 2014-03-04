using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Astra.DatabaseContext;
using Astra.Models;

namespace Astra.Repositories
{
  public class AuditRepository : IAuditRepository
  {
    private AstraContext _context = null;

    public AuditRepository(AstraContext context)
    {
      _context = context;
    }
    public void AddAuditRecord(AuditRecord audit)
    {
      if (audit!=null)
      {
        _context.AuditRecords.Add(audit);
        _context.SaveChanges();
      }
      
    }

    public void Dispose()
    {
      // Do nothing
    }
  }
  public interface IAuditRepository : IDisposable
  {
    void AddAuditRecord(AuditRecord audit);
  }
}