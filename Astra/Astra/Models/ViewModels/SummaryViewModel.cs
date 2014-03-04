using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MTUtil.DateTimes;

namespace Astra.Models.ViewModels
{
  public class SummaryViewModel
  {
    private Resource _resource;
    public Resource Resource
    {
      get { return _resource; }
      set { _resource = value; }
    }

    private int _copiesAvailable;
    public int CopiesAvailable
    {
      get { return _copiesAvailable; }
      set { _copiesAvailable = value; }
    }

    private bool _userHasResourceCheckedOut;
    public bool UserHasResourceCheckedOut
    {
      get { return _userHasResourceCheckedOut; }
      set { _userHasResourceCheckedOut = value; }
    }

    private DateTime _userCheckedOutResourceDate;
    public DateTime UserCheckedOutResourceDate
    {
      get { return _userCheckedOutResourceDate; }
      set { _userCheckedOutResourceDate = value; }
    }
    private Comment _comment;

    public Comment Comment
    {
      get { return _comment; }
      set { _comment = value; }
    }

    private bool _userHasResourceReserved;
    public bool UserHasResourceReserved
    {
      get { return _userHasResourceReserved; }
      set { _userHasResourceReserved = value; }
    }
    
    public bool CanBeCheckedOut
    {
      get
      {
        if (Resource == null)
        {
          return false;
        }
        else
        {
          return CopiesAvailable != 0;
        }
      }
    }

    public string AdminNote
    {
      get {
            if (string.IsNullOrWhiteSpace(_resource.AdminNote))
              return "none";
            else 
                return _resource.AdminNote;
          }
      set { _resource.AdminNote = value; }
    }

    public SummaryViewModel()
    {
      Resource = null;
      CopiesAvailable = 0;
      UserHasResourceCheckedOut = false;
      UserHasResourceReserved = false;
      UserCheckedOutResourceDate = DateTimeUtils.NullDate;
    }
  }
}