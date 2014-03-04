using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MTUtil.DateTimes;

namespace Astra.Helper
{
  public class SummaryReservationHistoryItem : ISummaryHistoryItem
  {
    private DateTime _actionTimestamp;
    public DateTime ActionTimestamp
    {
      get { return DateTimeUtils.UTCToLocal(_actionTimestamp); }
      set { _actionTimestamp = value; }
    }

    private string _actionLabel;
    public string ActionLabel
    {
      get { return _actionLabel; }
      set { _actionLabel = value; }
    }

    public SummaryReservationHistoryItem(DateTime timestamp, string label)
    {
      //store date for check in or out
      ActionTimestamp = timestamp;
      ActionLabel = label;
    }

    public int CompareTo(object obj)
    {
      if (!(obj is ISummaryHistoryItem))
      {
        throw new Exception("Invalid comparison attempted.");
      }
      return this.ActionTimestamp.CompareTo((obj as ISummaryHistoryItem).ActionTimestamp);
    }
  }
}