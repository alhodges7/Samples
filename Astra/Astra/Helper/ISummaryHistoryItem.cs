using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Astra.Helper
{
  public interface ISummaryHistoryItem : IComparable
  {
    DateTime ActionTimestamp { get; }
    string ActionLabel { get; }
  }
}