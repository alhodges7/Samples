using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Astra.Models
{
  public class UsersListViewModel
  {
    public string MID { get; set; }

    private string _firstName;
    public string FirstName
    {
      get { return _firstName; }
      set
      {
        _firstName = value.ToLower();
        _firstName = char.ToUpper(value[0]) + value.Substring(1).ToLower();
      }
    }

    public DateTime CheckedInDate { get; set; }

    public DateTime CheckedOutDate { get; set; }
  }
}
