using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;
using WebMatrix.WebData;
using Astra.CompositeRepository;
using Astra.Models;
using Astra.Controllers;

namespace Astra.Helper
{
  /// <summary>
  /// AstraMembershipProvider was created as a wrapper class for the SimpleMembershipProvider 
  /// because Astra uses MID, not MINDTREE\MID as the UserName.
  /// 
  /// ValidateUser(string login, string password) can also do an AD login should that be necessary
  /// </summary>
  public class AstraMembershipProvider : SimpleMembershipProvider
  {
    public AstraMembershipProvider() : base()
    {
      StripOffDomainNames = true;
    }

    public bool StripOffDomainNames { get; set; }


    public override bool ChangePassword(string username, string oldPassword, string newPassword)
    {
      if (StripOffDomainNames)
      {
        username = MembershipHelper.StripOffDomain(username);
      }
      return base.ChangePassword(username, oldPassword, newPassword);
    }

    public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
    {
      if (StripOffDomainNames)
      {
        username = MembershipHelper.StripOffDomain(username);
      }
      return base.ChangePasswordQuestionAndAnswer(username, password, newPasswordQuestion, newPasswordAnswer);
    }

    public override bool ConfirmAccount(string userName, string accountConfirmationToken)
    {
      if (StripOffDomainNames)
      {
        userName = MembershipHelper.StripOffDomain(userName);
      }
      return base.ConfirmAccount(userName, accountConfirmationToken);
    }

    public override string CreateAccount(string userName, string password, bool requireConfirmationToken)
    {
      if (StripOffDomainNames)
      {
        userName = MembershipHelper.StripOffDomain(userName);
      }
      return base.CreateAccount(userName, password, requireConfirmationToken);
    }

    public override void CreateOrUpdateOAuthAccount(string provider, string providerUserId, string userName)
    {
      if (StripOffDomainNames)
      {
        userName = MembershipHelper.StripOffDomain(userName);
      }
      base.CreateOrUpdateOAuthAccount(provider, providerUserId, userName);
    }

    public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
    {
      if (StripOffDomainNames)
      {
        username = MembershipHelper.StripOffDomain(username);
      }
      return base.CreateUser(username, password, email, passwordQuestion, passwordAnswer, isApproved, providerUserKey, out status);
    }

    public override string CreateUserAndAccount(string userName, string password, bool requireConfirmation, IDictionary<string, object> values)
    {
      if (StripOffDomainNames)
      {
        userName = MembershipHelper.StripOffDomain(userName);
      }
      return base.CreateUserAndAccount(userName, password, requireConfirmation, values);
    }

    public override bool DeleteAccount(string userName)
    {
      if (StripOffDomainNames)
      {
        userName = MembershipHelper.StripOffDomain(userName);
      }
      return base.DeleteAccount(userName);
    }

    public override bool DeleteUser(string username, bool deleteAllRelatedData)
    {
      if (StripOffDomainNames)
      {
        username = MembershipHelper.StripOffDomain(username);
      }
      return base.DeleteUser(username, deleteAllRelatedData);
    }

    public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
    {
      if (StripOffDomainNames)
      {
        usernameToMatch = MembershipHelper.StripOffDomain(usernameToMatch);
      }
      return base.FindUsersByName(usernameToMatch, pageIndex, pageSize, out totalRecords);
    }

    public override string GeneratePasswordResetToken(string userName, int tokenExpirationInMinutesFromNow)
    {
      if (StripOffDomainNames)
      {
        userName = MembershipHelper.StripOffDomain(userName);
      }
      return base.GeneratePasswordResetToken(userName, tokenExpirationInMinutesFromNow);
    }

    public override ICollection<OAuthAccountData> GetAccountsForUser(string userName)
    {
      if (StripOffDomainNames)
      {
        userName = MembershipHelper.StripOffDomain(userName);
      }
      return base.GetAccountsForUser(userName);
    }

    public override DateTime GetCreateDate(string userName)
    {
      if (StripOffDomainNames)
      {
        userName = MembershipHelper.StripOffDomain(userName);
      }
      return base.GetCreateDate(userName);
    }

    public override DateTime GetLastPasswordFailureDate(string userName)
    {
      if (StripOffDomainNames)
      {
        userName = MembershipHelper.StripOffDomain(userName);
      }
      return base.GetLastPasswordFailureDate(userName);
    }

    public override string GetPassword(string username, string answer)
    {
      if (StripOffDomainNames)
      {
        username = MembershipHelper.StripOffDomain(username);
      }
      return base.GetPassword(username, answer);
    }

    public override DateTime GetPasswordChangedDate(string userName)
    {
      if (StripOffDomainNames)
      {
        userName = MembershipHelper.StripOffDomain(userName);
      }
      return base.GetPasswordChangedDate(userName);
    }

    public override int GetPasswordFailuresSinceLastSuccess(string userName)
    {
      if (StripOffDomainNames)
      {
        userName = MembershipHelper.StripOffDomain(userName);
      }
      return base.GetPasswordFailuresSinceLastSuccess(userName);
    }

    public override MembershipUser GetUser(string username, bool userIsOnline)
    {
      if (StripOffDomainNames)
      {
        username = MembershipHelper.StripOffDomain(username);
      }
      return base.GetUser(username, userIsOnline);
    }

    public int GetUserId(string userName)
    {
      if (StripOffDomainNames)
      {
        userName = MembershipHelper.StripOffDomain(userName);
      }
      return base.GetUserId(userName);
    }

    public override bool IsConfirmed(string userName)
    {
      if (StripOffDomainNames)
      {
        userName = MembershipHelper.StripOffDomain(userName);
      }
      return base.IsConfirmed(userName);
    }

    public override string ResetPassword(string username, string answer)
    {
      if (StripOffDomainNames)
      {
        username = MembershipHelper.StripOffDomain(username);
      }
      return base.ResetPassword(username, answer);
    }

    public override bool UnlockUser(string userName)
    {
      if (StripOffDomainNames)
      {
        userName = MembershipHelper.StripOffDomain(userName);
      }
      return base.UnlockUser(userName);
    }

    /// <summary>
    /// The ValidateUser(string login, string password) method was overwriden so that when the 
    /// WebSecurity.Login(string userName, string password, bool persistCookie = false)
    /// is called and it automatically calls default membership provider.ValidateUser(string login, string password) 
    /// we can support an Active Directory login, but at the same time, allow custom passwords in case we want to use 
    /// this application on the world wide web some day.
    /// 
    /// The specific funtioning of this is that if WebSecurity.Login(userName, password, bool persistCookie = false)
    /// is called with the AccountController.AD_LOGIN_PASSWORD password, then it will attempt to do an 
    /// Active Directory login.  
    /// 
    /// The userName in WebSecurity.Login(userName, password, bool persistCookie = false) will be displayed on 
    /// the home page index later and cannot be changed here.
    /// 
    /// It is very important that the Active Directory domain checking is done in this function.  Checking the domain 
    /// before calling this function was considered --- but that would leave a potential security loophole where
    /// a future programmer could pass in the our domain name even if this code is not actually runing inside our domain.
    /// </summary>
    /// <param name="login">user name (MID)</param>
    /// <param name="password">password - AccountController.AD_LOGIN_PASSWORD triggers AD Login</param>
    /// <returns>success boolean</returns>
    public override bool ValidateUser(string login, string password)
    {
      if (password == AccountController.AD_LOGIN_PASSWORD)
      {
        string ADName = WindowsIdentity.GetCurrent().Name.ToUpper();

        string domainName = string.Empty, userMID = string.Empty;
        bool currentMIDExists = false;

        if (ADName.Contains(@"\"))
        {
          string[] stringArray = ADName.Split('\\');
          domainName = stringArray[0];
          userMID = stringArray[1];

          using (var repositories = new ScopedCompositeRepository())
          {
            currentMIDExists = repositories.Repositories.UserProfileRepository.MIDExists(userMID);
          }
        }

        if (domainName == "MINDTREE" && currentMIDExists)
          return true;
        else
          return false;
      }
      else
        return base.ValidateUser(login, password);
    }


   public bool IsValidEmail(string strIn)
    {
      // Return true if strIn is in valid e-mail format.
      return Regex.IsMatch(strIn, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
    }

  }
}