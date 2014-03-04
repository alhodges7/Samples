using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Astra.Models.ViewModels
{
  public class RatingsListViewModel
  {
    private int _ratingId;
    private int _resourceId;
    private string _userMID;
    private double _rating;
    private string _name;

    public RatingsListViewModel(Rating input)
    {

      _ratingId = input.RatingId;
      _resourceId = input.ResourceID;
      _userMID = input.UserMID;
      _rating = input.UserRating;
    }

    public void AddName(UserProfile up)
    {
      _name = up.FirstName + " " + up.LastName;
    }
     
    public int RatingId
    {
      get { return _ratingId; }
      set { _ratingId = value; }
    }

   
    public  string Name
    {
      get { return _name; }
      set { _name = value; }
    }

    public int ResourceID
    {
      get { return _resourceId; }
      set { _resourceId = value; }
    }

    public string UserMID
    {
      get { return _userMID; }
      set { _userMID = value; }
    }

    public double UserRating
    {
      get { return _rating; }
      set { _rating = value; }
    }
  }
}