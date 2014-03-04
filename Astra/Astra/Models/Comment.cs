using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Astra.Models
{
  public class Comment : AstraBaseModel
  {
    private int _commentId;
    private int _resourceId;
    private string _userMID;
    private string _comment;
    private Rating _rating;
    private Resource _resource;
 
    [Key]
    public int CommentId
    {
      get { return _commentId; }
      set { _commentId = value; }
    }

    [ForeignKey("ResourceID")]
    public Resource Resource
    {
      get { return _resource; }
      set { _resource = value; }
    }
    [Required]
    public int ResourceID
    {
      get { return _resourceId; }
      set { _resourceId = value; }
    }

    [Required]
    public string UserMID
    {
      get { return _userMID; }
      set { _userMID = value; }
    }


    public string UserComment
    {
      get { return _comment; }
      set { _comment = value; }
    }

    public virtual Rating Rating
    {
      get { return _rating; }
      set { _rating = value; }
    }

  }
}