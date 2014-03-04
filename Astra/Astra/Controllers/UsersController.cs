using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Astra.Helper;
using Astra.Models;
using Astra.Repositories;
using WebMatrix.WebData;
using Astra.Controllers.Shared;
using Astra.Models.ViewModels;
using Astra.CompositeRepository;
namespace Astra.Controllers
{
  public class UsersController : BaseController
  {
    public ActionResult Index()
    {
      using (var repositories = new ScopedCompositeRepository().Repositories)
      {
        SelectList availableResourcesList = new SelectList(repositories.ResourceRepository.GetAvailableResources(), "ResourceID", "Title", "Available Resources");
        ViewBag.AvailableResources = availableResourcesList;
        ViewBag.Message = TempData["Message"];
        return View(repositories.UserProfileRepository.All);
      }
    }

    public ActionResult Select([Bind(Prefix = "MID")] string mid = "")
    {
      using (var repositories = new ScopedCompositeRepository().Repositories)
      {
        UserProfile userProfile = repositories.UserProfileRepository.FindProfileByMID(mid);
        MyCheckOutsViewModel selectedUserCheckoutsObj = new MyCheckOutsViewModel(mid);
        ViewBag.FullName = userProfile.FirstName + " " + userProfile.LastName;
        selectedUserCheckoutsObj.ActiveCheckOuts = repositories.CheckoutRepository.GetActiveCheckOutsForUser(mid);
        selectedUserCheckoutsObj.CheckOutHistory = repositories.CheckoutRepository.GetCheckOutHistoryForUser(mid);
        ViewBag.Message = "Check outs for: " + userProfile.FirstName + " " + userProfile.LastName;

        return View(selectedUserCheckoutsObj);
      }
    }

    public ActionResult CheckIn(int resourceId)
    {
      string username = MembershipHelper.CurrentUserName();
      using (var repositories = new ScopedCompositeRepository().Repositories)
      {
        repositories.CheckoutRepository.CheckIn(resourceId, username, MembershipHelper.UserHasRoles(username,
          new string[] { MembershipHelper.ROLE_ADMIN }));

        if (Roles.IsUserInRole(MembershipHelper.CurrentUserName(), MembershipHelper.ROLE_ADMIN))
        {
          return RedirectToAction("Select", "Users", new { MID = username });
        }

        return RedirectToAction("Select", "Users", new { MID = username });
      }
    }

    #region Helper Methods
    /// <summary>
    /// Gets Username from Database based on MID
    /// This function is static so it can be called from class name 
    /// w/out instantiation of a UsersController object.
    /// </summary>
    /// <param name="mid">MID of any employee</param>
    /// <returns>Firstname + space + Lastname unless no name is found, then 
    /// the same input mid string is returned</returns>
    public static string GetUserNameByMID(string mid = "")
    {
      using (var repositories = new ScopedCompositeRepository().Repositories)
      {
        UserProfile userProfile = repositories.UserProfileRepository.FindProfileByMID(mid);
        if (userProfile == null)
          return mid;

        string fullUserName =  userProfile.FirstName + " " + userProfile.LastName;
        if (fullUserName == null || fullUserName == " ")
          return mid;
        else
          return fullUserName;
      }
    }

    /// <summary>
    /// Gets MID strings for all librarians and administrators
    /// </summary>
    public static List<string> GetLibrarianAdminMIDList()
    {
      using (var repositories = new ScopedCompositeRepository().Repositories)
      {
        return repositories.UserProfileRepository.FindlLibrarianAdminMIDList();
      }
    }
    #endregion
  }
}
