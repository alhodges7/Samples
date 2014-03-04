using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.Routing;
using Astra.Helper;
using Astra.Models;
using Astra.Repositories;
using WebMatrix.WebData;
using Astra.Controllers.Shared;
using Astra.CompositeRepository;
using Astra.Models.ViewModels;

namespace Astra.Controllers
{
  public class CheckOutsController : BaseController
  {
    //empty default constructor
    public CheckOutsController() : base()
    {
    }

    public ActionResult CheckOutHistory(int resourceId)
    {
      using (var repositories = new ScopedCompositeRepository().Repositories)
      {
        Resource resourceObj = repositories.ResourceRepository.Find<Resource>(resourceId);
        List<CheckOut> checkoutHistory = repositories.CheckoutRepository.GetCheckOutHistoryForResource(resourceId);
        List<ResourceCheckOutHistoryViewModel> chkOutHistViewModelList = new List<ResourceCheckOutHistoryViewModel>();
        foreach (var checkout in checkoutHistory)
        {
          ResourceCheckOutHistoryViewModel chkOutHistViewModel = new ResourceCheckOutHistoryViewModel(checkout);
          chkOutHistViewModelList.Add(chkOutHistViewModel);
        }

        ViewBag.Title = resourceObj.Title;
        ViewBag.Copies = resourceObj.Copies;
        ViewBag.CopiesAvailable = resourceObj.Copies - repositories.CheckoutRepository.GetActiveCheckOutsForResource(resourceId).Count();

        return View(chkOutHistViewModelList);
      }
    }

    public ActionResult CheckOut(int resourceId = 0)
    {
      bool result = false;
      using (var repositories = new ScopedCompositeRepository().Repositories)
      {
          result = repositories.CheckoutRepository.CheckOut(resourceId, MembershipHelper.CurrentUserName(), MembershipHelper.CurrentUserIsAdminOrLibrarian());
      }

      return new JsonResult
      {
        Data = new { result = result }
      };
    }

    // Post Create method overload for admin checkout
    [ActionName("AdminCheckOut")]
    [HttpPost]
    public ActionResult CheckOut(string mId, int resourceId)
    {
      using (var repositories = new ScopedCompositeRepository().Repositories)
      {
        repositories.CheckoutRepository.CheckOut(resourceId, mId, MembershipHelper.CurrentUserIsAdminOrLibrarian());

        return new JsonResult()
        {
          Data = new { result = true }
        };
      }
    }

    [ActionName("AdminCheckIn")]
    [HttpPost]
    public ActionResult CheckIn(string mId, int resourceId)
    {
      using (var repositories = new ScopedCompositeRepository().Repositories)
      {
        repositories.CheckoutRepository.CheckIn(resourceId, mId, MembershipHelper.CurrentUserIsAdminOrLibrarian());

        return new JsonResult()
        {
          Data = new { result = true }
        };
      }
    }

    public ActionResult CheckIn(int resourceId)
    {
      using (var repositories = new ScopedCompositeRepository().Repositories)
      {
          repositories.CheckoutRepository.CheckIn(resourceId, MembershipHelper.CurrentUserName(), MembershipHelper.CurrentUserIsAdminOrLibrarian());
      }

      return new JsonResult()
      {
        Data = new { result = true },
        JsonRequestBehavior = JsonRequestBehavior.AllowGet,
        
      };
    }

    protected override void Dispose(bool disposing)
    {
      base.Dispose(disposing);
    }
  }
}




