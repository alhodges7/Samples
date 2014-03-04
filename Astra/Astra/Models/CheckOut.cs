using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MTUtil;
using MTUtil.TypeManagement;
using MTUtil.DateTimes;

namespace Astra.Models
{
  public enum CheckOutStatus
  {
    Unspecified = 0,
    CheckedOut = 1,
    CheckedIn = 2,
    Lost = 3,
    NotApplicable = 99
  }


  public class CheckOut : AstraBaseModel
  {    
    private int _checkOutID;
    private int _resourceID;
    private Resource _resource;
    private string _userMID;
    private DateTime _dateCheckedOut = DateTime.UtcNow;
    private DateTime _dateCheckedIn = DateTimeUtils.NullDate;
    private int _daysAllowed;
    private int _intCheckOutStatus = 0;
    
    [Key]
    public int CheckOutID
    {
      get { return _checkOutID; }
      set { _checkOutID = value; }
    }

    [Required]
    public int ResourceID
    {
      get { return _resourceID; }
      set { _resourceID = value; }
    }

    [ForeignKey("ResourceID")]
    public Resource Resource
    {
      get { return _resource; }
      set { _resource = value; }
    }

    [Required]
    public string UserMID
    {
      get { return _userMID; }
      set { _userMID = value; }
    }

    public CheckOutStatus CheckOutStatus
    {
      get { return TypeUtils.ToEnum<CheckOutStatus>(_intCheckOutStatus); }
      set { _intCheckOutStatus = value.GetHashCode(); }
    }

    public DateTime DateCheckedOut
    {
      get { return _dateCheckedOut; }
      set { _dateCheckedOut = value; }
    }

    public DateTime DateCheckedIn
    {
      get { return _dateCheckedIn; }
      set { _dateCheckedIn = value; }
    }

    public int DaysAllowed
    {
      get { return _daysAllowed; }
      set { _daysAllowed = value; }
    }

    private int IntCheckOutStatus
    {
      get { return _intCheckOutStatus; }
      set { _intCheckOutStatus = value; }
    }       
  }
}