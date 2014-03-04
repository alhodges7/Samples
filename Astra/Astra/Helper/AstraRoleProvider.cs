using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using WebMatrix.WebData;
using Astra.Controllers;

namespace Astra.Helper
{
  /// <summary>
  /// AstraRoleProvider was created as a wrapper class for the SimpleRoleProvider 
  /// because Astra uses MID, not MINDTREE\MID as the UserName.
  /// </summary>
  public class AstraRoleProvider : SimpleRoleProvider
  {
    public AstraRoleProvider() : base()
    {
      StripOffDomainNames = true;
    }

    public bool StripOffDomainNames { get; set; }


    public override void AddUsersToRoles(string[] usernames, string[] roleNames)
    {
      if (StripOffDomainNames)
      {
        for (int i = 0; i < usernames.Length; i++)
        {
          usernames[i] = MembershipHelper.StripOffDomain(usernames[i]);
        }
      }
      base.AddUsersToRoles(usernames, roleNames);
    }

    public override bool IsUserInRole(string username, string roleName)
    {
      if (StripOffDomainNames)
      {
        username = MembershipHelper.StripOffDomain(username);
      }
      return base.IsUserInRole(username, roleName);
    }

    public bool IsUserInRole(string roleName)
    {
      string username = WebSecurity.CurrentUserName;
      if (StripOffDomainNames)
      {
        username = MembershipHelper.StripOffDomain(username);
      }
      return base.IsUserInRole(username, roleName);
    }

    public override string[] FindUsersInRole(string roleName, string usernameToMatch)
    {
      if (StripOffDomainNames)
      {
        usernameToMatch = MembershipHelper.StripOffDomain(usernameToMatch);
      }
      return base.FindUsersInRole(roleName, usernameToMatch);
    }

    public override string[] GetRolesForUser(string username)
    {
      if (StripOffDomainNames)
      {
        username = MembershipHelper.StripOffDomain(username);
      }
      return base.GetRolesForUser(username);
    }

    public string[] GetRolesForUser()
    {
      string username = WebSecurity.CurrentUserName; //GetRolesForUser, that is to say, for the current user
      if (StripOffDomainNames)
      {
        username = MembershipHelper.StripOffDomain(username);
      }
      return base.GetRolesForUser(username);
    }

    public override string[] GetUsersInRole(string roleName)
    {
      string[] usersStringArray = base.GetUsersInRole(roleName);
      if (StripOffDomainNames)
      {
        for (int i = 0; i < usersStringArray.Length; i++)
        {
          usersStringArray[i] = MembershipHelper.StripOffDomain(usersStringArray[i]);
        }
      }
      return usersStringArray;
    }

    public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
    {
      if (StripOffDomainNames)
      {
        for (int i = 0; i < usernames.Length; i++)
        {
          usernames[i] = MembershipHelper.StripOffDomain(usernames[i]);
        }
      }
      base.RemoveUsersFromRoles(usernames, roleNames);
    }

  }
}