using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Astra.Models.ViewModels
{
  public class ResourceCheckOutHistoryViewModel
  {
    public ResourceCheckOutHistoryViewModel()
    {

    }
    public ResourceCheckOutHistoryViewModel(CheckOut chkOut)
    {
      this.UserMID = chkOut.UserMID;
      this.ResourceId = chkOut.ResourceID;
      this.CheckOutId = chkOut.CheckOutID;
      this.Status = chkOut.CheckOutStatus;
      this.CheckedOutDate = chkOut.DateCheckedOut;
      this.CheckedInDate = chkOut.DateCheckedIn;
      using (var repository = new CompositeRepository.ScopedCompositeRepository().Repositories.UserProfileRepository) 
      {
        UserProfile user = repository.FindProfileByMID(chkOut.UserMID);
        this.CheckedOutBy = user.FirstName + " " + user.LastName;
      }
    }
    public int ResourceId { get; set; }
    public int CheckOutId { get; set; }
    public string UserMID { get; set; }
    public string CheckedOutBy { get; set; }
    public CheckOutStatus Status { get; set; }
    public DateTime CheckedOutDate { get; set; }
    public DateTime CheckedInDate { get; set; }

  }
}