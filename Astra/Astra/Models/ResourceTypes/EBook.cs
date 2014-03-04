using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Astra.Models.ResourceTypes
{
  public class EBook : Book
  {
    private string _url;

    [StringLength(255, MinimumLength = 0)]
    public string Url
    {
      get { return _url; }
      set { _url = value; }
    }
  }
}