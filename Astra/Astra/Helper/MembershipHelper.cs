using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Security;
using WebMatrix.WebData;
using Astra.CompositeRepository;
using Astra.Logging;
using Astra.Models;

namespace Astra.Helper
{
    public static class MembershipHelper
    {
        private const string ROOT_USER = "root";
        public const string ROLE_BASIC_USER = "User";
        public const string ROLE_ADMIN = "Administrator";
        public const string ROLE_LIBRARIAN = "Librarian";

        public static bool FormsAuthMode { get; set; }

        public static string CurrentWindowsMID()
        {
          string ADName = WindowsIdentity.GetCurrent().Name.ToUpper();
          if (ADName.Contains(@"\"))
          {
              ADName =  ADName.Split('\\')[1];
          }
          return ADName;
        }

        public static string StripOffDomain(string domainName)
        {
          if (domainName.Contains(@"\"))
          {
            domainName = domainName.Split('\\')[1];
          }
          return domainName;
        }

        public static bool MIDExists(string mid)
        {
          if (mid.Contains(@"\"))
          {
            mid = mid.Split('\\')[1];
          }
          using (var repositories = new ScopedCompositeRepository())
          {
            return repositories.Repositories.UserProfileRepository.MIDExists(mid);
          }
        }

        public static bool ActiveDirectoryMIDExists()
        {
          string ADName = WindowsIdentity.GetCurrent().Name.ToUpper();
          if (ADName.Contains(@"\"))
          {
            ADName = ADName.Split('\\')[1];
          }

          using (var repositories = new ScopedCompositeRepository())
          {
            return repositories.Repositories.UserProfileRepository.MIDExists(ADName);
          }
        }

        public static bool UserMIDIsActive(string mid)
        {
          if (mid.Contains(@"\"))
          {
            mid = mid.Split('\\')[1];
          }

          using (var repository = new ScopedCompositeRepository().Repositories.UserProfileRepository)
          {
            return repository.MIDIsActive(mid);
          }
        }
      
        public static string CurrentUserName()
        {
          string userName = null;
          try
          {
            userName = WebSecurity.CurrentUserName;
            if (userName.Contains(@"\"))
            {
              userName = userName.Split('\\')[1];
            }
          }
          catch
          {
            return null;
          }

          return userName;
        }


        public static string HttpContextMID()
        {
          string userName = null;
          try
          {
            userName = HttpContext.Current.User.Identity.Name.ToUpper();
            if (userName.Contains(@"\"))
            {
              userName = userName.Split('\\')[1];
            }
          }
          catch
          {
            return null;
          }

          return userName;
        }


        /// <summary>
        /// This function registers a new astra user from the active directory information
        /// </summary>
        /// <param name="userProfile"></param>
        public static void RegisterWindowsMember(UserProfile userProfile = null)
        {
          if (userProfile == null)
          {
            // Creater a new user, import from AD
            string userMID = StripOffDomain(HttpContext.Current.User.Identity.Name.ToUpper());
              //= CurrentWindowsMID()

            using (var context = new PrincipalContext(ContextType.Domain, "MINDTREE"))
            {
              using (var user = UserPrincipal.FindByIdentity(context, userMID))
              {
                if (user != null)
                {
                  userProfile = new UserProfile()
                  {
                    MID = userMID,
                    FirstName = user.GivenName,
                    LastName = user.Surname,
                    Email = user.EmailAddress,
                    IsActive = true
                  };

                }
              }
            }
          }

          //"WindowsAuthenticationDefaultPassword" is a Generic Password 
          //for registering a new user w/out them entering a password
          WebSecurity.CreateUserAndAccount(userProfile.MID.ToUpper(), "WindowsAuthenticationDefaultPassword");
          using (var repositories = new ScopedCompositeRepository())
          {
            repositories.Repositories.UserProfileRepository.AddUser(userProfile);
          }
          Roles.AddUserToRole(userProfile.MID, MembershipHelper.ROLE_BASIC_USER);
        }




        /// <summary>
        /// This function gets registgration information from the Active Directory
        /// and return it in a RegisterModel obect
        /// Error condition: the new RegisterModel.MID = string.Emtpy
        /// </summary>
        /// <returns>new RegisterModel object</returns>
        public static RegisterModel GetActiveDirectoryRegistrationInfo()
        {
          RegisterModel newUserRegistration = new RegisterModel();

          // Creater a new user, import from AD
          using (var context = new PrincipalContext(ContextType.Domain, "MINDTREE"))
          {
            string ADName = WindowsIdentity.GetCurrent().Name.ToUpper();
            string domainName = string.Empty, userMID = string.Empty;

            if (ADName.Contains(@"\"))
            {
              string[] stringArray = ADName.Split('\\');
              domainName = stringArray[0];
              userMID = stringArray[1];
            }

            //you can only register inside the mindtree domain
            //this prevents mid impersonation
            if (domainName != "MINDTREE")
            {
              newUserRegistration.MID = string.Empty;
              return newUserRegistration;
            }

            using (var user = UserPrincipal.FindByIdentity(context, userMID))
            {
              if (user != null)
              {
                newUserRegistration.MID = userMID;
                newUserRegistration.FirstName = user.GivenName;
                newUserRegistration.LastName = user.Surname;
                newUserRegistration.Email = user.EmailAddress;
              }
              else
              {
                newUserRegistration.MID = string.Empty;
              }
            }
          }
          return newUserRegistration;
        }


        public static bool ValidateWindowsUser(string userMID = null)
        {
          UserProfile userProfile = new UserProfile();

          if (userMID == null) userMID = CurrentWindowsMID();

          if (!WebSecurity.IsAuthenticated)
          {
            // validate the user has been imported
            using (var repositories = new ScopedCompositeRepository())
            {
              userProfile = repositories.Repositories.UserProfileRepository.FindProfileByMID(userMID);
            }

            if (userProfile != null)
            {
              //Authenticate the user, they exist in the database already
              //"WindowsAuthenticationDefaultPassword" is the Generic Password 
              //used when a user is autoregistered by going to the website in Windows Validation mode
              //this will fail if the user registered back when the website was in Forms mode 
              //and has a password other than "WindowsAuthenticationDefaultPassword".
              //However, this function is not currently in use
              if (WebSecurity.Login(userMID, "WindowsAuthenticationDefaultPassword", false)) 
              {
                return true;
              }
              else
              {
                return false;
              }
            }
            else
            {
              // Creater a new user, import from AD
              using (var context = new PrincipalContext(ContextType.Domain, "MINDTREE"))
              {
                using (var user = UserPrincipal.FindByIdentity(context, userMID))
                {
                  if (user != null)
                  {
                    userProfile = new UserProfile()
                    {
                      MID = userMID,
                      FirstName = user.GivenName,
                      LastName = user.Surname,
                      Email = user.EmailAddress,
                      IsActive = true
                    };

                    RegisterWindowsMember(userProfile);
                  }
                  else
                  {
                    MembershipHelper.FormsAuthMode = true;
                    return false;
                  }
                }
              }
            }
          }
          return true;
        }

        public static void SeedRoles()
        {
          AstraLogger.LogInfo(typeof(MembershipHelper), "Seeding role data...");

          var roles = Roles.Provider;

          if (!roles.RoleExists(ROLE_ADMIN))
          {
            roles.CreateRole(ROLE_ADMIN);
          }
          if (!roles.RoleExists(ROLE_LIBRARIAN))
          {
            roles.CreateRole(ROLE_LIBRARIAN);
          }
          if (!roles.RoleExists(ROLE_BASIC_USER))
          {
            roles.CreateRole(ROLE_BASIC_USER);
          }

          AstraLogger.LogInfo(typeof(MembershipHelper), "Membership data seeded successfully...");
        }


        public static void SeedMembership()
        {
            AstraLogger.LogInfo(typeof(MembershipHelper), "Seeding membership data...");

            var roles = Roles.Provider;
            var membership = Membership.Provider;

            if (!roles.RoleExists(ROLE_ADMIN))
            {
                roles.CreateRole(ROLE_ADMIN);
            }
            if (!roles.RoleExists(ROLE_LIBRARIAN))
            {
                roles.CreateRole(ROLE_LIBRARIAN);
            }
            if (!roles.RoleExists(ROLE_BASIC_USER))
            {
                roles.CreateRole(ROLE_BASIC_USER);
            }

            if (membership.GetUser(ROOT_USER, false) == null)
            {
              WebSecurity.CreateUserAndAccount(ROOT_USER, "mindtree#99");
            }

            if (!roles.GetRolesForUser(ROOT_USER).Contains(ROLE_ADMIN))
            {
                roles.AddUsersToRoles(new[] { ROOT_USER }, new[] { ROLE_ADMIN });
            }

            if (!roles.GetRolesForUser(ROOT_USER).Contains(ROLE_BASIC_USER))
            {
                roles.AddUsersToRoles(new[] { ROOT_USER }, new[] { ROLE_BASIC_USER });
            }

            // William's Account
            if (membership.GetUser("M1021311", false) == null)
            {
              WebSecurity.CreateUserAndAccount("M1021311", "WindowsAuthenticationDefaultPassword");
            }

            if (!roles.GetRolesForUser("M1021311").Contains(ROLE_ADMIN))
            {
              roles.AddUsersToRoles(new[] { "M1021311" }, new[] { ROLE_ADMIN });
            }

            if (!roles.GetRolesForUser("M1021311").Contains(ROLE_BASIC_USER))
            {
              roles.AddUsersToRoles(new[] { "M1021311" }, new[] { ROLE_BASIC_USER });
            }

			      // Ash's Account
			      if (membership.GetUser("M1021693", false) == null)
			      {
              WebSecurity.CreateUserAndAccount("M1021693", "mindtree#99");
			      }

            if (!roles.GetRolesForUser("M1021693").Contains(ROLE_ADMIN))
			      {
              roles.AddUsersToRoles(new[] { "M1021693" }, new[] { ROLE_ADMIN });
			      }

			      if (!roles.GetRolesForUser("M1021693").Contains(ROLE_BASIC_USER))
			      {
				      roles.AddUsersToRoles(new[] { "M1021693" }, new[] { ROLE_BASIC_USER });
			      }


            // Fernando's Account
            if (membership.GetUser("M1021310", false) == null)
            {
              WebSecurity.CreateUserAndAccount("M1021310", "password");
              if (!roles.GetRolesForUser("M1021310").Contains(ROLE_BASIC_USER))
              {
                roles.AddUsersToRoles(new[] { "M1021310" }, new[] { ROLE_BASIC_USER });
                roles.AddUsersToRoles(new[] { "M1021310" }, new[] { ROLE_LIBRARIAN });
              }
            }

            // Terry's Account
            if (membership.GetUser("M1020537", false) == null)
            {
              WebSecurity.CreateUserAndAccount("M1020537", "password");
              if (!roles.GetRolesForUser("M1020537").Contains(ROLE_BASIC_USER))
              {
                  roles.AddUsersToRoles(new[] { "M1020537" }, new[] { ROLE_BASIC_USER });
                  roles.AddUsersToRoles(new[] { "M1020537" }, new[] { ROLE_LIBRARIAN });
              }
            }

            // Dave's Account
            if (membership.GetUser("M1021022", false) == null)
            {
              WebSecurity.CreateUserAndAccount("M1021022", "password");
              if (!roles.GetRolesForUser("M1021022").Contains(ROLE_BASIC_USER))
              {
                roles.AddUsersToRoles(new[] { "M1021022" }, new[] { ROLE_BASIC_USER });
              }
            }

            if (membership.GetUser("M1021023", false) == null)
            {
              WebSecurity.CreateUserAndAccount("M1021023", "password");
              if (!roles.GetRolesForUser("M1021023").Contains(ROLE_BASIC_USER))
              {
                roles.AddUsersToRoles(new[] { "M1021023" }, new[] { ROLE_BASIC_USER });
              }
            }
            if (membership.GetUser("M1020707", false) == null)
            {
              WebSecurity.CreateUserAndAccount("M1020707", "password");
              if (!roles.GetRolesForUser("M1020707").Contains(ROLE_BASIC_USER))
              {
                  roles.AddUsersToRoles(new[] { "M1020707" }, new[] { ROLE_BASIC_USER });
              }
            }

            AstraLogger.LogInfo(typeof(MembershipHelper), "Membership data seeded successfully...");
        }

        public static bool UserHasRoles(string username, string[] roles)
        {
            if (string.IsNullOrEmpty(username))
                return false;

            var rolesProvider = Roles.Provider;

            string[] userRoles = rolesProvider.GetRolesForUser(username);
            foreach (string role in roles)
            {
                if (!userRoles.Contains(role))
                    return false;
            }

            return true;
        }

        public static bool CurrentUserIsAdminOrLibrarian()
        {
            return UserHasRoles(MembershipHelper.CurrentUserName(), new string[] { MembershipHelper.ROLE_LIBRARIAN })
              || UserHasRoles(MembershipHelper.CurrentUserName(), new string[] { MembershipHelper.ROLE_ADMIN });
        }

    }
}