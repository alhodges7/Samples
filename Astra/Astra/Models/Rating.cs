using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Astra.Models
{
  public class Rating : AstraBaseModel
  {
    private int _ratingId;
    private int _resourceId;
    private string _userMID;
    private double _rating;
    private Resource _resource;

    [Key]
    public int RatingId
    {
      get { return _ratingId; }
      set { _ratingId = value; }
    }

    [ForeignKey("ResourceID")]
    public virtual Resource Resource
    {
      get { return _resource; }
      set { _resource = value; }
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