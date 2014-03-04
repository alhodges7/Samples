using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Astra
{
  public class RouteConfig
  {
    public static void RegisterRoutes(RouteCollection routes)
    {
      routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
      routes.IgnoreRoute("{resource}.mti/{*pathInfo}");



      routes.MapRoute(
        "UXPError",
        "Error/UXPError/Msg/{errorMessageGuid}",
        new { controller = "Error", action = "UXPError", errorMessageGuid = "Unknown Error" }
      );

      routes.MapRoute(
        "ResetUserPassword",
        "Admin/ResetUserPassword/ProfileId/{userProfileId}",
        new { controller = "Admin", action = "ResetUserPassword", userProfileId = 0 }
      );

      routes.MapRoute(
        "NotActive",
        "Account/NotActive",
        new { controller = "Account", action = "NotActive", userProfileId = 0 }
      );

      routes.MapRoute(
          name: "Default",
          url: "{controller}/{action}/{id}",
          defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
      );
    }
  }
}