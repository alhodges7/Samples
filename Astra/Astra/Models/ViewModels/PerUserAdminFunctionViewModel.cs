using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Astra.Models.ViewModels;


namespace Astra.Models.ViewModels
{
  public class PerUserAdminFunctionViewModel
  {
    private int _userId;
    private string _userMID;
    private string _userFirstName;
    private string _userLastName;
    private string _userEmail;
    private string _userStatus;
    private List<CheckOut> _checkOutHistory;
    private List<Comment> _userComments;
    private List<Rating> _userRatings;
    private List<Reservation> _userReservations;
    private List<ResourceSuggestion> _userSuggestions;
    private List<PerUserResourceViewModel> _resources;


    #region Construcors
    public PerUserAdminFunctionViewModel()
    {
    }
    public PerUserAdminFunctionViewModel(UserProfile input, List<CheckOut> checkouts, List<Comment> comments, List<Rating> ratings, List<ResourceSuggestion> suggestions, IEnumerable<Reservation> reservations)
    {
      _userId = input.Id;
      _userMID = input.MID;
      _userFirstName = input.FirstName;
      _userLastName = input.LastName;
      _userEmail = input.Email;
      _userStatus = IsActive(input.IsActive);
      _checkOutHistory = checkouts;
      _userComments = comments;
      _userRatings = ratings;
      _userSuggestions = suggestions;
      _userReservations = reservations.ToList();
      _resources = FillUserResources(checkouts, comments, ratings, _userReservations);

    }
    #endregion

    # region Public Accessors
    public int UserId
    {
      get { return _userId; }
      set { _userId = value; }
    }

    public string UserMID
    {
      get { return _userMID; }
      set { _userMID = value; }
    }

    public string UserFirstName
    {
      get { return _userFirstName; }
      set { _userFirstName = value; }
    }

    public string UserLastName
    {
      get { return _userLastName; }
      set { _userLastName = value; }
    }

    public string UserEmail
    {
      get { return _userEmail; }
      set { _userEmail = value; }
    }

    public string UserStatus
    {
      get { return _userStatus; }
      set { _userStatus = value; }
    }

    public List<CheckOut> CheckOutHistory
    {
      get { return _checkOutHistory; }
      set { _checkOutHistory = value; }
    }

    public List<Comment> UserComments
    {
      get { return _userComments; }
      set { _userComments = value; }
    }

    public List<Rating> UserRatings
    {
      get { return _userRatings; }
      set { _userRatings = value; }
    }

    public List<Reservation> UserReservations
    {
      get { return _userReservations; }
      set { _userReservations = value; }
    }

    public List<ResourceSuggestion> UserSuggestions
    {
      get { return _userSuggestions; }
      set { _userSuggestions = value; }
    }

    public List<PerUserResourceViewModel> Resources
    {
      get { return _resources; }
      set { _resources = value; }
    }

    #endregion

    #region Helper Functions

    private string IsActive(bool status)
    {
      if (status)
      {
        return "Active";
      }
      return "Deactivated";

    }

    private List<PerUserResourceViewModel> FillUserResources(List<CheckOut> checkouts, List<Comment> comments, List<Rating> ratings, List<Reservation> reservations)
    {
      //Create UserResources based on Checkouts History
      List<PerUserResourceViewModel> resources = new List<PerUserResourceViewModel>();
      if (_checkOutHistory.Count != 0)
      {
        foreach (var resource in _checkOutHistory)
        {
          if (resources.Any(x => x.ResourceID == resource.ResourceID && x.UserMID == resource.UserMID))
          {
            PerUserResourceViewModel tempResource = resources.FirstOrDefault(x => x.ResourceID == resource.ResourceID
                                                                 && x.UserMID == resource.UserMID
                                                                 && x.UserCheckOut.DateCheckedOut < resource.DateCheckedOut);
            if (tempResource != null)
            {
              resources.Remove(tempResource);
            }

          }
          else
          {
            resources.Add(new PerUserResourceViewModel(resource));
          }

        }

      }


      //Add Comments to UserResources and add new UserResource based on comment
      foreach (var comment in comments)
      {
        bool IsResourceFound = false;
        if (resources.Count != 0)
        {
          foreach (var resource in resources)
          {
            if (resource.ResourceID == comment.ResourceID)
            {
              resource.UserComment = new Comment()
                                      {
                                        CommentId = comment.CommentId,
                                        CreatedOn = comment.CreatedOn,
                                        Rating = comment.Rating,
                                        ResourceID = comment.ResourceID,
                                        UserComment = comment.UserComment,
                                        UserMID = comment.UserMID
                                      };
              if (resource.UserComment.Rating == null)
              {
                resource.UserComment.Rating = new Rating();
              }
              IsResourceFound = true;
              break;
            }
          }
        }
        if (!IsResourceFound)
        {
          resources.Add(new PerUserResourceViewModel(comment.ResourceID, comment.UserMID));
          if (comment.Rating == null)
          {
            comment.Rating = new Rating();
          }
          resources.Last().UserComment = comment;
        }

      }
      // Add ratings to UserResources and create UserResource based on ratings
      foreach (var rating in ratings)
      {
        bool IsRated = false;
        if (resources.Count != 0)
        {
          foreach (var resource in resources)
          {
            if (resource.ResourceID == rating.ResourceID)
            {
              resource.UserComment.Rating = rating;
              IsRated = true;
              break;
            }
          }
        }
        if (!IsRated)
        {
          resources.Add(new PerUserResourceViewModel(rating.ResourceID, rating.UserMID));
          resources.Last().UserComment.Rating = rating;
        }
      }
      // Add Reservations to UserResources and create UserResource based on Reservations
      foreach (var reservation in reservations)
      {
        bool IsReserved = false;
        if (resources.Count != 0)
        {
          foreach (var resource in resources)
          {
            if (resource.ResourceID == reservation.ResourceID)
            {
              resource.UserReservation = reservation;
              IsReserved = true;
              break;
            }
          }
        }
        if (!IsReserved)
        {
          resources.Add(new PerUserResourceViewModel(reservation.ResourceID, reservation.UserMID));
          resources.Last().UserReservation = reservation;
        }
      }


      return resources;
    }

    #endregion
  }
}