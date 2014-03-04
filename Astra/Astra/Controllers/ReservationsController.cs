using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Astra.CompositeRepository;
using WebMatrix.WebData;
using Astra.Helper;

namespace Astra.Controllers
{
    public class ReservationsController : Controller
    {
        [Authorize]
        public ActionResult ReserveBook(int resourceId)
        {
          using (var repository = new ScopedCompositeRepository().Repositories.ReservationRepository)
          {
              repository.ReserveBook(MembershipHelper.CurrentUserName(), resourceId);
          }

          return new JsonResult() { Data = new { result = true } };
        }

        [Authorize]
        public ActionResult CancelReservation(int resourceId)
        {
          using (var repository = new ScopedCompositeRepository().Repositories.ReservationRepository)
          {
              repository.CancelReservation(MembershipHelper.CurrentUserName(), resourceId);
          }

          return new JsonResult() { Data = new { result = true } };
        }
    }
}
