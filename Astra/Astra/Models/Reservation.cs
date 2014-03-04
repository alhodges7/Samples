using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using MTUtil.DateTimes;

namespace Astra.Models
{
  public enum ReservationStatus
  {
    Reserved,
    Accepted,
    Cancelled,
    Undefined
  }

  public class Reservation : AstraBaseModel
  {
    private int _reservationId;
    private int _resourceID;
    private Resource _resource;
    private string _userMID;
    private DateTime _dateReserved = DateTime.UtcNow;
    private int _daysAllowed;
    private int _intCheckOutStatus = 0;


    [Key]
    public int ReservationID
    {
      get { return _reservationId; }
      set { _reservationId = value; }
    }

    private ReservationStatus _reservationStatus;
    public ReservationStatus ReservationStatus
    {
      get { return _reservationStatus; }
      set { _reservationStatus = value; }
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

    public DateTime DateReserved
    {
      get { return _dateReserved; }
      set { _dateReserved = value; }
    }
  }
}