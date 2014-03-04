using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Astra.Helper
{
  public class AstraExceptionWrapper
  {
    private Guid _Guid;
    private DateTime _ErrorDateUTC = DateTime.UtcNow;
    private string _MID;
    private string _ErrorMessage;
    private string _StackTrace;
    private string _ReferrerUrl;

    public Guid Guid
    {
      get { return _Guid; }
      set { _Guid = value; }
    }

    public DateTime ErrorDateUTC
    {
      get { return _ErrorDateUTC; }
      set { _ErrorDateUTC = value; }
    }

    public string MID
    {
      get { return _MID; }
      set { _MID = value; }
    }

    public string ErrorMessage
    {
      get { return _ErrorMessage; }
      set { _ErrorMessage = value; }
    }

    public string StackTrace
    {
      get { return _StackTrace; }
      set { _StackTrace = value; }
    }

    public string ReferrerUrl
    {
      get { return _ReferrerUrl; }
      set { _ReferrerUrl = value; }
    }


  }
}