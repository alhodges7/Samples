using System;
using System.Collections.Generic;
using System.Linq;
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

namespace Astra
{
  // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
  // visit http://go.microsoft.com/?LinkId=9394801

  public class MvcApplication : System.Web.HttpApplication
  {
    
    protected void Application_Start()
    {
      Task NotificationService = new Task(() => new NotificationServices.NotificationServicesScheduler(), NotificationServices.CancellationTokenLibrary.NotificationsCancelationToken);
      ModelBinders.Binders.Add(typeof(Resource), new ResourceModelBinder());
      AreaRegistration.RegisterAllAreas();
      WebApiConfig.Register(GlobalConfiguration.Configuration);
      FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
      RouteConfig.RegisterRoutes(RouteTable.Routes);
      BundleConfig.RegisterBundles(BundleTable.Bundles);
      AuthConfig.RegisterAuth();

      AstraContext db = new AstraContext();
      List<KeyWord> list = db.KeyWords.ToList<KeyWord>();
      int x = list.Count;

      WebMatrix.WebData.WebSecurity.InitializeDatabaseConnection("AstraContext", "aspnet_Users", "ID", "MID", true);
      MembershipHelper.SeedRoles();

      log4net.Config.XmlConfigurator.Configure();
      log4net.ILog log = log4net.LogManager.GetLogger(this.GetType());
      log.Info("Log4Net initialized...");

      db.PurgeOldLogs();

      NotificationService.Start();
      //Detect Windows/Forms authentication
      System.Configuration.Configuration config = WebConfigurationManager.OpenWebConfiguration("~");
      SystemWebSectionGroup configSection = (SystemWebSectionGroup)config.GetSectionGroup("system.web");
      AuthenticationSection auth = configSection.Authentication;
      MembershipHelper.FormsAuthMode = (auth.Mode == AuthenticationMode.Forms) ? true : false;
    }

    protected void Application_End()
    {
      NotificationServices.CancellationTokenLibrary.NotificationsCancellationTokenSource.Cancel();
      log4net.LogManager.Shutdown();
    }

    protected void Application_AuthenticateRequest()
    {
      //In Forms Mode, you need to check if a user has been deactivated, but WebSecurity.IsAuthenticated
      //has not yet changed to false & logged them out
      if (MembershipHelper.FormsAuthMode == true && WebSecurity.IsAuthenticated)
      {
        using (var repository = new ScopedCompositeRepository().Repositories.UserProfileRepository)
        {
          if (!repository.MIDIsActive(MembershipHelper.StripOffDomain(WebSecurity.CurrentUserName)))
          {
            FormsAuthentication.SignOut();
            FormsAuthentication.RedirectToLoginPage();
          }
        }
      }

      //WebSecurity.IsAuthenticated also prevents a null reference errors for 
      //Context.Request.Url.AbsolutePath & HttpContext.Current.User.Identity.Name & WebSecurity.CurrentUserName

      //In Windows Validation Mode, you just test if the user is not active manually
      //but you have to watch out for an infinite loop error where the system can try 
      //to send the deactivated user to the "NotActive" page over & over, hence the 
      //check  for "Account/NotActive" in the coditional test
      if (MembershipHelper.FormsAuthMode == false && WebSecurity.IsAuthenticated && Context.Request.Url.AbsolutePath.Contains("Account/NotActive") == false)
      {
        using (var repository = new ScopedCompositeRepository().Repositories.UserProfileRepository)
        {
          string mid = MembershipHelper.StripOffDomain(HttpContext.Current.User.Identity.Name.ToUpper());
          if (repository.MIDExists(mid) && repository.MIDIsActive(mid) == false)
          {
            Response.RedirectToRoute("NotActive");
          }
        }
      }
      
    }

    protected void Application_BeginRequest()
    {


    }

    protected void Application_PostAuthenticateRequest()
    {

    }


    protected void Session_Start(object sender, EventArgs e)
    {
      //If this user has not yet been registered, register them in the Session Start event - but only if we're using 
      //Windows Authentication --- in Forms Login mode, it's up to users to register themselves
      if (!MembershipHelper.FormsAuthMode && !MembershipHelper.MIDExists(HttpContext.Current.User.Identity.Name.ToUpper()))
      {
        MembershipHelper.RegisterWindowsMember();
      }
    }
  }
}