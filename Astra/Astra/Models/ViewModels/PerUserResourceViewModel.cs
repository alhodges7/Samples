using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Astra.CompositeRepository;

namespace Astra.Models.ViewModels
{
  public class PerUserResourceViewModel
  {
    private int _resourceID;
    private string _title;
    private string _author;
    private double _avgRating;
    private string _isbn10;
    private string _isbn13;
    private string _userMID;
    private int _copies = 1;
    private int _checkedOutCopies;
    private CheckOut _userCheckout;
    private Comment _userComment;
    private Reservation _userReservation;

    #region Constructors
    public PerUserResourceViewModel()
    {
      //Default
    }

    public PerUserResourceViewModel(int resourceId, string userMID)
    {
      using (var resource = new ScopedCompositeRepository().Repositories.ResourceRepository)
      {
        Resource temp = resource.Find<Resource>(resourceId);
        _userCheckout = new CheckOut() { CheckOutStatus = CheckOutStatus.Unspecified, CreatedOn = DateTime.MinValue, DateCheckedIn = DateTime.MinValue, DateCheckedOut = DateTime.MinValue };
        _userComment = new Comment() { CreatedOn = DateTime.MinValue, Rating = new Rating() { CreatedOn = DateTime.MinValue }, ResourceID = resourceId, UserMID = userMID };
        _userReservation = new Reservation() { CreatedOn=DateTime.MinValue, DateReserved = DateTime.MinValue, ReservationStatus = ReservationStatus.Undefined };
        _userMID = userMID;
        _resourceID = temp.ResourceID;
        _title = temp.Title;
        _avgRating = temp.Rating;
      }
    }

    public PerUserResourceViewModel(CheckOut input)
    {
      _userMID = input.UserMID;
      _resourceID = input.ResourceID;
      _title = input.Resource.Title;
      _avgRating = input.Resource.Rating;
      _userCheckout = new CheckOut
      {
        CheckOutID = input.CheckOutID,
        CheckOutStatus = input.CheckOutStatus,
        CreatedByMID = input.CreatedByMID,
        CreatedOn = input.CreatedOn,
        DateCheckedIn = input.DateCheckedIn,
        DateCheckedOut = input.DateCheckedOut,
        DaysAllowed = input.DaysAllowed,
        LastModifiedByMID = input.LastModifiedByMID,
        LastModifiedOn = input.LastModifiedOn,
        ResourceID = input.ResourceID,
        UserMID = input.UserMID
      };
      _userComment = new Comment() { Rating = new Rating() };
      _userReservation = new Reservation() { CreatedOn = DateTime.MinValue, ReservationStatus = ReservationStatus.Undefined, UserMID = input.UserMID, ResourceID = input.ResourceID };
    }

    #endregion

    #region Public Accessors

    public int ResourceID
    {
      get { return _resourceID; }
      set { _resourceID = value; }
    }

    public string Title
    {
      get { return _title; }
      set { _title = value; }
    }

    public string Author
    {
      get { return _author; }
      set { _author = value; }
    }

    public double AvgRating
    {
      get { return _avgRating; }
      set { _avgRating = value; }
    }

    public string ISBN10
    {
      get
      { return _isbn10; }
      set
      { _isbn10 = value; }
    }

    public string ISBN13
    {
      get
      { return _isbn13; }
      set
      { _isbn13 = value; }
    }

    public string UserMID
    {
      get
      { return _userMID; }
      set
      { _userMID = value; }
    }

    public int Copies
    {
      get { return _copies; }
      set { _copies = value; }
    }

    public int CheckedOutCopies
    {
      get { return _checkedOutCopies; }
      set { _checkedOutCopies = value; }
    }

    public CheckOut UserCheckOut
    {
      get { return _userCheckout; }
      set { _userCheckout = value; }
    }

    public Comment UserComment
    {
      get { return _userComment; }
      set { _userComment = value; }
    }
    public Reservation UserReservation
    {
      get { return _userReservation; }
      set { _userReservation = value; }
    }

    #endregion
  }


}