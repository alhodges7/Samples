using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Security;
using Astra.CompositeRepository;
using Astra.Models;
using Astra.Helper;

namespace Astra.Filters
{
  public class AuditAttribute : ActionFilterAttribute
  {
    //Our value to handle our AuditingLevel
    public int AuditingLevel { get; set; }

    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
        
        //Stores the Request in an Accessible object
        var request = filterContext.HttpContext.Request;

        //Generate the appropriate key based on the user's Authentication Cookie
        //This is overkill as you should be able to use the Authorization Key from
        //Forms Authentication to handle this. 
        string sessionIdentifier;
        if (request.Cookies[FormsAuthentication.FormsCookieName] == null)
        {
            sessionIdentifier = "99999";
        }
        else
        {
            sessionIdentifier = string.Join(string.Empty, MD5.Create().ComputeHash(Encoding.ASCII.GetBytes(request.Cookies[FormsAuthentication.FormsCookieName].Value)).Select(s => s.ToString("x2")));
        }
        //Generate an audit
        AuditRecord audit = new AuditRecord()
        {
            SessionID = sessionIdentifier,
            AuditId = Guid.NewGuid(),
            IpAddress = request.ServerVariables["HTTP_X_FORWARDED_FOR"] ?? request.UserHostAddress,
            UrlAccessed = request.RawUrl,
            CreatedOn = DateTime.UtcNow,
            Mid = request.IsAuthenticated ? MembershipHelper.StripOffDomain(filterContext.HttpContext.User.Identity.Name) : "Anonymous",
            Data = SerializeRequest(request)
        };

        //Stores the Audit in the Database
        using (var repository = new ScopedCompositeRepository().Repositories.AuditRepository)
        {
            repository.AddAuditRecord(audit);
            base.OnActionExecuting(filterContext);
        }
    }

    private string SerializeRequest(HttpRequestBase request)
    {
      switch (AuditingLevel)
      {
        //No Request Data will be serialized
        case 0:
        default:
          return string.Empty;
        //Basic Request Serialization - just stores Data
        case 1:
          return Json.Encode(new { request.Cookies, request.Headers, request.Files });
        //Middle Level - Customize to your Preferences
        case 2:
          return Json.Encode(new { request.Cookies, request.Headers, request.Files, request.Form, request.QueryString, request.Params });
        //Highest Level - Serialize the entire Request object
        case 3:
          //We can't simply just Encode the entire request string due to circular references as well
          //as objects that cannot "simply" be serialized such as Streams, References etc.
          return Json.Encode(new { request.Cookies, request.Headers, request.Files, request.Form, request.QueryString, request.Params });
      }
    }
  }
}