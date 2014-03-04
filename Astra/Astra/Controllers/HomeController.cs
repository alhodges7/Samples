using System;
using System.Web.Mvc;
using System.Linq;
using System.Collections.Generic;
using Astra.Models.ViewModels;
using Astra.Controllers.Shared;
using Astra.Models;
using Astra.CompositeRepository;
using MTUtil.IO;
using Astra.Helper;
using WebMatrix.WebData;
using PagedList;

namespace Astra.Controllers
{
	public class HomeController : BaseController
	{
    private const int PAGE_SIZE = 5;    
    
		public ActionResult Index()
		{
      ViewBag.NewItemsList = "New Additions";
      ViewBag.RecommendedList = "Recommendations";

      IndexPagerViewModel view = new IndexPagerViewModel();
      view.RecentlyViewed = view.GetRecentlyViewed(PAGE_SIZE);
      view.RecentlyAddedItems = view.GetRecentlyAddedItems();
      view.Recommendations = view.GetRecommendations();
      return View(view);			
		}

    [HttpPost]
    public ActionResult Redirect()
    {
      return View("Index");
    }

		public ActionResult About()
		{
      FileVersion fv = new FileVersion(new System.IO.FileInfo(this.GetType().Assembly.Location));
      ViewBag.CurrentVersion = fv.ToString();
			return View();
		}

    [Authorize]
    public ActionResult MyCheckOuts()
    {
      string userMid = MembershipHelper.CurrentUserName();
      MyCheckOutsViewModel checkouts = new MyCheckOutsViewModel(userMid);
      using (var repositories = new ScopedCompositeRepository().Repositories)
      {
        checkouts.CheckOutHistory = repositories.CheckoutRepository.GetCheckOutHistoryForUser(userMid);
        checkouts.ActiveCheckOuts = repositories.CheckoutRepository.GetActiveCheckOutsForUser(userMid);
        checkouts.ActiveReservations = repositories.ReservationRepository.GetAllReservationsForUser(userMid);
        return View(checkouts);
      }
    }
	}
}


