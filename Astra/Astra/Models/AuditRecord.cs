using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Astra.Models
{
  public class AuditRecord 
  {
    private string _sessionID;
    private Guid _auditId;
    private string _internetProtocolAddress;
    private string _mid;
    private string _urlAccessed;
    private string _data;
    private DateTime _createdOn;

    public AuditRecord() { }

    [Key]
    public Guid AuditId
    {
      get { return _auditId; }
      set { _auditId = value; }
    }
    public string SessionID
    {
      get{return _sessionID;}
      set { _sessionID = value; }
    }
    public string IpAddress
    {
      get { return _internetProtocolAddress; }
      set { _internetProtocolAddress = value; }
    }
    public string Mid
    {
      get { return _mid; }
      set { _mid = value; }
    }
    public string UrlAccessed
    {
      get { return _urlAccessed; }
      set { _urlAccessed = value; }
    }
    public string Data
    {
      get { return _data; }
      set { _data = value; }
    }
    public DateTime CreatedOn
    {
      get { return _createdOn; }
      set { _createdOn = value; }
    }

  }
}