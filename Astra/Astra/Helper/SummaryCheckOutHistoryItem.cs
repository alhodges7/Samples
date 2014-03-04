using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using Astra.Models;
using MTUtil.DateTimes;

namespace Astra.Helper
{
  public class SummaryCheckOutHistoryItem : ISummaryHistoryItem
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

    private string _ModifiedBy;
    public string ModifiedBy
    {
      get { return _ModifiedBy; }
      set { _ModifiedBy = value; }
    }

    private string _fullName;
    public string FullName
    {
      get { return _fullName; }
      set { _fullName = value; }
    }

    public SummaryCheckOutHistoryItem(DateTime timestamp, string label, string fullname,string modifiedby)
    {
      //store date for check in or out
      ActionTimestamp = timestamp;
      
      if (Roles.IsUserInRole(modifiedby, MembershipHelper.ROLE_ADMIN) || Roles.IsUserInRole(modifiedby, MembershipHelper.ROLE_LIBRARIAN))
      {
        // if admin or librarian created check out, store who did
        if (Roles.IsUserInRole(modifiedby, MembershipHelper.ROLE_ADMIN))
        {
          this.ModifiedBy = MembershipHelper.ROLE_ADMIN;
          ActionLabel = label + fullname + " by " + this.ModifiedBy;
        }
        else
        {
          this.ModifiedBy = MembershipHelper.ROLE_LIBRARIAN;
          ActionLabel = label + fullname +  " by " + this.ModifiedBy;
        }
      }
      else
      {
        //otherwise the user checked it out for his/her self
        if (label.Contains("in"))
        {
          ActionLabel = "Checked in by " + fullname;
        }
        else
        {
          ActionLabel = "Checked out by " + fullname;
        }
       
      }
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