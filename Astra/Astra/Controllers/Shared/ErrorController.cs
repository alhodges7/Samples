using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.Mvc;
using Astra.Helper;
using MTUtil.DateTimes;

namespace Astra.Controllers.Shared
{
  public class ErrorController : Controller
  {
    private static Dictionary<Guid, AstraExceptionWrapper> _RecentExceptions = null;

    public static Dictionary<Guid, AstraExceptionWrapper> RecentExceptions
    {
      get
      {
        if (_RecentExceptions == null)
          _RecentExceptions = new Dictionary<Guid, AstraExceptionWrapper>();
        return _RecentExceptions;
      }
    }

    public static void StoreException(AstraExceptionWrapper wrapper)
    {
      ErrorController.RecentExceptions[wrapper.Guid] = wrapper;
      ErrorController.PurgeOldExceptions();
    }

    public static AstraExceptionWrapper FetchException(Guid wrapperGuid)
    {
      return ErrorController.RecentExceptions[wrapperGuid];
    }

    public static void PurgeOldExceptions()
    {
      List<Guid> guidsToPurge = new List<Guid>();
      foreach (AstraExceptionWrapper exceptionWrapper in ErrorController.RecentExceptions.Values)
      {
        if (DateTimeUtils.ElapsedMinutes(exceptionWrapper.ErrorDateUTC) >= 30)
          guidsToPurge.Add(exceptionWrapper.Guid);
      }

      foreach (Guid guid in guidsToPurge)
      {
        ErrorController.RecentExceptions.Remove(guid);
      }

    }

    [HttpGet]
    public ViewResult UXPError(string errorMessageGuid)
    {
      Guid errorGuid = Guid.Parse(errorMessageGuid);
      AstraExceptionWrapper exceptionWrapper = ErrorController.FetchException(errorGuid);

      ViewBag.showErrorDetail = false;

#if DEBUG
      ViewBag.showErrorDetail = true;
#endif

      return View(exceptionWrapper);
    }
  }
}