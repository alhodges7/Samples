using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Astra.Models.ViewModels
{
  public class MyCheckOutsViewModel
  {

    public MyCheckOutsViewModel(string userMid)
    {
      this.UserMid = userMid;
    }

    public List<CheckOut> ActiveCheckOuts { get; set; }
    public List<CheckOut> CheckOutHistory { get; set; }

    public IEnumerable<Reservation> ActiveReservations { get; set; }

    public string UserMid { get; set; }
  }
}