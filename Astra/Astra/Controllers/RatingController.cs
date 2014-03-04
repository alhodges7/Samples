using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Astra.Models;
using Astra.Controllers.Shared;
using Astra.CompositeRepository;
using Astra.Models.ViewModels;
using Astra.Helper;

namespace Astra.Controllers
{
  public class RatingController : BaseController
  {
    [Authorize]
    public ActionResult UserRating(int resourceId)
    {
      double userrating = 0;
      using (var repository = new ScopedCompositeRepository().Repositories.RatingsRepository)
      {
          userrating = repository.GetUserRating(resourceId, MembershipHelper.CurrentUserName());
      }
      ViewBag.userRating = userrating;
      ViewBag.resourceId = resourceId;

      return PartialView("../Rating/_Rate");
    }

    [HttpPost]
    [Authorize]
    public ActionResult RateResource(double userRating, int resourceId)
    {
      string userMid = MembershipHelper.CurrentUserName();
      using (var repository = new ScopedCompositeRepository().Repositories.RatingsRepository)
      {
        repository.RateResource(userMid, resourceId, userRating);

        double avgRating = repository.GetAverageRatingForResource(resourceId);

        return Json(avgRating, JsonRequestBehavior.AllowGet);
      }
    }

    [Authorize]
    public ActionResult RatingList(int resourceId)
    {
      List<RatingsListViewModel> ratingList = new List<RatingsListViewModel>();
      using (var resource = new ScopedCompositeRepository().Repositories.RatingsRepository)
      {
        foreach (var item in resource.GetAllRatingsForResource(resourceId))
        {
          // create EACH RatingsListViewModel and add to ratingList
          ratingList.Add(new RatingsListViewModel(item));
        }
      }

      using (var resource = new ScopedCompositeRepository().Repositories.UserProfileRepository)
      {
        // find name of each user who rated resource
        foreach (var item in ratingList)
        {
          item.AddName(resource.FindProfileByMID(item.UserMID));
        }
      }

      return PartialView("../Admin/_RatingsList", ratingList);
    }

    protected override void Dispose(bool disposing)
    {
      base.Dispose(disposing);
    }
  }
}
