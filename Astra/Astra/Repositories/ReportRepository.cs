using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Data;
using WebMatrix.WebData;
using Astra.DatabaseContext;
using Astra.Models;
using Astra.Helper;


namespace Astra.Repositories
{
  public class ReportRepository : IReportRepository
  {
    private AstraContext _context = null;

    public ReportRepository(AstraContext context)
    {
      _context = context;
    }

    public void Dispose()
    {
      _context.Dispose();
    }

    public Report Find(int reportId)
    {
      return _context.Reports.Find(reportId);
    }

    public List<Report> All()
    {
      return _context.Reports.OrderBy(r => r.Title).ToList<Report>();
    }

    public void InsertOrUpdate(Report report)
    {
      if (report.ReportID == default(int))
      {
        // New entity
        _context.Reports.Add(report);
      }
      else
      {
        // Existing entity
        _context.Entry(report).State = EntityState.Modified;
      }
    }
    
    public void EditReport(int ReportId)
    {
      var search = _context.Reports.Where(x => x.ReportID == ReportId);
      if (!search.Any())
      {
        throw new Exception(string.Format("Could not locate Report with id {0}.", ReportId));
      }

      var Report = search.First();          

      _context.SaveChanges();
    }

    public void RemoveReport(int reportId)
    {
      var searchReport = _context.Reports.Where(x => x.ReportID == reportId);

      if (!searchReport.Any())
      {
        throw new Exception("Could not locate Report with id " + reportId);
      }

      Report removeReport = searchReport.First();
      if (Roles.IsUserInRole(MembershipHelper.CurrentUserName(), MembershipHelper.ROLE_ADMIN))
      {
        _context.Reports.Remove(removeReport);
        _context.SaveChanges();
      }
    }

   
  }

  public interface IReportRepository : IDisposable
  {
    List<Report> All();
    void InsertOrUpdate(Report report);
    void EditReport(int ReportId);    
    void RemoveReport(int ReportId);
    Report Find(int reportId);
  }
}