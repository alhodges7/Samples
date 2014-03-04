using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;

using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using System.Security.Principal;
using WebMatrix.WebData;
using Astra.CompositeRepository;
using Astra.DatabaseContext;
using Astra.Helper;
using Astra.Models;
using Astra.Controllers;

using Astra.Helper;
using log4net;

namespace Astra.Controllers.Shared
{
  public class BaseController : Controller
  {
    public BaseController()
    {

    }

    protected override void OnException(ExceptionContext filterContext)
    {

      Guid errorGuid = System.Guid.NewGuid();
      string errorMsg = "An unexpected Error occured [" + errorGuid.ToString().ToUpper() + "]";

      // Log the error that occurred.
      Exception e = filterContext.Exception;
      ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
      log.Fatal(errorMsg, e);

      AstraExceptionWrapper exceptionWrapper = new AstraExceptionWrapper();
      exceptionWrapper.Guid = errorGuid;
      exceptionWrapper.ErrorMessage = e.Message;
      exceptionWrapper.StackTrace = e.StackTrace;
      exceptionWrapper.MID = MembershipHelper.CurrentUserName();      
      ErrorController.StoreException(exceptionWrapper);      

      // Output a nice error page      
      filterContext.ExceptionHandled = true;
      Response.Redirect("~/Error/UXPError/Msg/" + errorGuid.ToString().ToUpper());
    }
        
  }
}