using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Security.Principal;
using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;
using Astra.Filters;
using Astra.Models;
using Astra.Helper;
using Astra.Controllers.Shared;
using MTUtil.TypeManagement;
using Astra.CompositeRepository;


namespace Astra.Controllers
{
  [Authorize]
  [InitializeSimpleMembership]
  public class AccountController : BaseController
  {
    public const string UserRole = "User";
    public const string AD_LOGIN_PASSWORD = "AstraIntranetSkeletonKey#99"; //only used if you call WebSecurity.Login while in Windows Validation mode

    [AllowAnonymous]
    public ActionResult Login(string returnUrl)
    {
      if (Request.IsAuthenticated || WebSecurity.IsAuthenticated)
      {
        return RedirectToLocal(returnUrl);
      }

      if (Url.IsLocalUrl(returnUrl) && !string.IsNullOrEmpty(returnUrl))
      {
        if (Request.UrlReferrer != null)
        {
          string querystring = Request.UrlReferrer.ToString();
          if (querystring.Contains('?'))
          {
            returnUrl += querystring.Substring(querystring.IndexOf('?'));
          }
        }

        ViewBag.ReturnUrl = returnUrl;
      }
      HttpCookie authCookie = HttpContext.Request.Cookies.Get("Mid");

      if (authCookie != null && MembershipHelper.FormsAuthMode)
      {
        FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(authCookie.Value);
        if (TypeUtils.ToBool(ticket.UserData) == true)
        {
          return View(new LoginModel { RememberMe = true, MID = ticket.Name });
        }
      }
      return View();
    }


    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public ActionResult Login(LoginModel model, string returnUrl)
    {
      UserProfile profile = null;
      using (var repositories = new ScopedCompositeRepository())
      {
        profile = repositories.Repositories.UserProfileRepository.FindProfileByMID(model.MID);
      }

      if (ModelState.IsValid && WebSecurity.Login(model.MID, model.Password, persistCookie: model.RememberMe))
      {

        if (profile != null && profile.IsActive)
        {
          FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, model.MID, DateTime.UtcNow,
          DateTime.UtcNow.AddDays(30), true, model.RememberMe.ToString(), FormsAuthentication.FormsCookiePath);

          //encrypt ticket  //create cookie
          Response.Cookies.Add(new HttpCookie("Mid", FormsAuthentication.Encrypt(ticket)));
          if (returnUrl.Contains("Register"))
          {
            //if user clicks log in from register page, redirect home
            returnUrl = "/";
          }
          else if (returnUrl.Contains("AdvSearch"))
          {
            returnUrl = "/Resources";
          }
          return RedirectToLocal(returnUrl);
        }
        else
        {
          // User is not active.
          ModelState.AddModelError(string.Empty, "This user has been deactivated.");
          if (Url.IsLocalUrl(returnUrl) && !string.IsNullOrEmpty(returnUrl))
          {
            ViewBag.ReturnUrl = returnUrl;
          }
          return View(model);
        }
      }

      // If we got this far, something failed, redisplay form
      ModelState.AddModelError(string.Empty, "The MID or password provided is incorrect.");
      if (Url.IsLocalUrl(returnUrl) && !string.IsNullOrEmpty(returnUrl))
      {
        ViewBag.ReturnUrl = returnUrl;
      }
      return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult LogOff()
    {
      WebSecurity.Logout();
      return RedirectToAction("Index", "Home");
    }

    [AllowAnonymous]
    public ActionResult Register()
    {
      if (Request.IsAuthenticated)
      {
        return RedirectToLocal("/");
      }
      return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public ActionResult Register(RegisterModel model)
    {
      if (ModelState.IsValid)
      {
        // We want all MID to be displayed with an upper-case "M"
        model.MID = model.MID.ToUpper(); 

        // Attempt to register the user
        try
        {
          WebSecurity.CreateUserAndAccount(model.MID, model.Password);

          WebSecurity.Login(model.MID, model.Password);


          UserProfile userProfile = new UserProfile();

          userProfile.MID = model.MID;
          userProfile.FirstName = model.FirstName;
          userProfile.LastName = model.LastName;
          userProfile.Email = model.Email;

          using (var repositories = new ScopedCompositeRepository())
          {
            repositories.Repositories.UserProfileRepository.AddUser(userProfile);
          }
          Roles.AddUserToRole(model.MID, MembershipHelper.ROLE_BASIC_USER);
          return RedirectToAction("Index", "Home");
        }
        catch (MembershipCreateUserException e)
        {
          ModelState.AddModelError(string.Empty, ErrorCodeToString(e.StatusCode));
        }
      }

      // If we got this far, something failed, redisplay form
      return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Disassociate(string provider, string providerUserId)
    {
      string ownerAccount = OAuthWebSecurity.GetUserName(provider, providerUserId);
      ManageMessageId? message = null;

      // Only disassociate the account if the currently logged in user is the owner
      string currentUserName = MembershipHelper.StripOffDomain(User.Identity.Name);
      if (ownerAccount == currentUserName)
      {
        // Use a transaction to prevent the user from deleting their last login credential
        using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.Serializable }))
        {
          bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(currentUserName));
          if (hasLocalAccount || OAuthWebSecurity.GetAccountsFromUserName(currentUserName).Count > 1)
          {
            OAuthWebSecurity.DeleteAccount(provider, providerUserId);
            scope.Complete();
            message = ManageMessageId.RemoveLoginSuccess;
          }
        }
      }

      return RedirectToAction("ManagePassword", new { Message = message });
    }

    public ActionResult ManageProfile()
    {
      return View();
    }

    public ActionResult BasicInfo()
    {
      string mid = MembershipHelper.CurrentUserName();
      using (var repository = new ScopedCompositeRepository())
      {
        UserProfile userProfile = repository.Repositories.UserProfileRepository.FindProfileByMID(mid);
        return View(userProfile);
      }
    }

    [HttpPost]
    public ActionResult BasicInfo(UserProfile userProfile)
    {
      if (!ModelState.IsValid)
        return RedirectToAction("ManageProfile");

      using (var repositories = new ScopedCompositeRepository())
      {
        repositories.Repositories.UserProfileRepository.Update(userProfile);
      }
      return RedirectToAction("ManageProfile");
    }


    public ActionResult HistoryCheckOuts(UserProfile userProfile)
    {
      return View();
    }

    public ActionResult ManagePassword(ManageMessageId? message)
    {
      ViewBag.StatusMessage =
          message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
          : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
          : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
          : string.Empty;

      string currentUserName = MembershipHelper.StripOffDomain(User.Identity.Name);
      ViewBag.HasLocalPassword = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(currentUserName));
      ViewBag.ReturnUrl = Url.Action("ManagePassword");
      return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult ManagePassword(LocalPasswordModel model)
    {
      string currentUserName = MembershipHelper.StripOffDomain(User.Identity.Name);
      bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(currentUserName));
      ViewBag.HasLocalPassword = hasLocalAccount;
      ViewBag.ReturnUrl = Url.Action("ManagePassword");
      if (hasLocalAccount)
      {
        if (ModelState.IsValid)
        {
          // ChangePassword will throw an exception rather than return false in certain failure scenarios.
          bool changePasswordSucceeded;
          try
          {
            changePasswordSucceeded = WebSecurity.ChangePassword(currentUserName, model.OldPassword, model.NewPassword);
          }
          catch (Exception)
          {
            changePasswordSucceeded = false;
          }

          if (changePasswordSucceeded)
          {
            return RedirectToAction("ManagePassword", new { Message = ManageMessageId.ChangePasswordSuccess });
          }
          else
          {
            ModelState.AddModelError(string.Empty, "The current password is incorrect or the new password is invalid.");
          }
        }
      }
      else
      {
        // User does not have a local password so remove any validation errors caused by a missing
        // OldPassword field
        ModelState state = ModelState["OldPassword"];
        if (state != null)
        {
          state.Errors.Clear();
        }

        if (ModelState.IsValid)
        {
          try
          {
            WebSecurity.CreateAccount(currentUserName, model.NewPassword);
            return RedirectToAction("ManagePassword", new { Message = ManageMessageId.SetPasswordSuccess });
          }
          catch (Exception e)
          {
            ModelState.AddModelError(string.Empty, e);
          }
        }
      }

      // If we got this far, something failed, redisplay form
      return View(model);
    }

    [AllowAnonymous]
    public ActionResult NotActive()
    {
      return View();
    }

    #region Helpers
    private ActionResult RedirectToLocal(string returnUrl)
    {
      if (Url.IsLocalUrl(returnUrl))
      {
        return Redirect(returnUrl);
      }
      else
      {
        return RedirectToAction("Index", "Home");
      }
    }

    public enum ManageMessageId
    {
      ChangePasswordSuccess,
      SetPasswordSuccess,
      RemoveLoginSuccess,
    }

    public UserProfile GetCurrentUserProfile
    {
      get
      {
        using (var repository = new ScopedCompositeRepository())
        {
          UserProfile userProfile = repository.Repositories.UserProfileRepository.FindProfileByMID(MembershipHelper.CurrentUserName());
          return userProfile;
        }
      }
    }

    internal class ExternalLoginResult : ActionResult
    {
      public ExternalLoginResult(string provider, string returnUrl)
      {
        Provider = provider;
        ReturnUrl = returnUrl;
      }

      public string Provider { get; private set; }
      public string ReturnUrl { get; private set; }

      public override void ExecuteResult(ControllerContext context)
      {
        OAuthWebSecurity.RequestAuthentication(Provider, ReturnUrl);
      }
    }

    private static string ErrorCodeToString(MembershipCreateStatus createStatus)
    {
      // See http://go.microsoft.com/fwlink/?LinkID=177550 for
      // a full list of status codes.
      switch (createStatus)
      {
        case MembershipCreateStatus.DuplicateUserName:
          return "An account for this MID already exists. Did you forget your password?";

        case MembershipCreateStatus.DuplicateEmail:
          return "An account for this e-mail address already exists. Please enter a different e-mail address.";

        case MembershipCreateStatus.InvalidPassword:
          return "The password provided is invalid. Please enter a valid password value.";

        case MembershipCreateStatus.InvalidEmail:
          return "The e-mail address provided is invalid. Please check the value and try again.";

        case MembershipCreateStatus.InvalidAnswer:
          return "The password retrieval answer provided is invalid. Please check the value and try again.";

        case MembershipCreateStatus.InvalidQuestion:
          return "The password retrieval question provided is invalid. Please check the value and try again.";

        case MembershipCreateStatus.InvalidUserName:
          return "The user name provided is invalid. Please check the value and try again.";

        case MembershipCreateStatus.ProviderError:
          return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

        case MembershipCreateStatus.UserRejected:
          return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

        default:
          return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
      }
    }
    #endregion
  }
}
