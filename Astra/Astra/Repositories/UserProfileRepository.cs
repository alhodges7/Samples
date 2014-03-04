using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Data.Entity;
using System.Web.Security;
using Astra.DatabaseContext;
using Astra.Helper;
using Astra.Models;
using WebMatrix.WebData;
using Astra.CompositeRepository;

namespace Astra.Repositories
{
	public class UserProfileRepository : IUserProfileRepository
	{
    private AstraContext _context = null;

    public UserProfileRepository(AstraContext context)
    {
      _context = context;
    }

    public IEnumerable<UserProfile> All
    {
      get
      {
        return _context.UserProfiles.ToList();
      }
    }

    public IEnumerable<UserProfile> AllIncluding(Expression<Func<UserProfile, bool>> filter = null, string includeProperties = "")
    {
      IQueryable<UserProfile> query = _context.UserProfiles;

      if (filter != null)
      {
        query = query.Where(filter);
      }

      foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
      {
        query = query.Include(includeProperty);
      }
      return query.ToList();
    }

    public UserProfile Find(int id)
    {
      return _context.UserProfiles.Find(id);
    }

    public UserProfile FindProfileByMID(string mid)
    {
      UserProfile userProfile = _context.UserProfiles.Where(up => up.MID == mid).FirstOrDefault();
      return userProfile;
    }

    public bool MIDExists(string mid)
    {
      return _context.UserProfiles.Where(up => up.MID == mid).Count() > 0 ;
    }

    public bool MIDIsActive(string mid)
    {
      UserProfile userProfile = _context.UserProfiles.Where(up => up.MID == mid).FirstOrDefault();
      if (userProfile == null)
        return false;
      else
        return userProfile.IsActive;
    }

    public void ToggleUserActiveState(string mid)
    {
      UserProfile userProfile = _context.UserProfiles.Where(up => up.MID == mid).FirstOrDefault();
      if (userProfile.IsActive)
      {
        userProfile.IsActive = false;
      }
      else
      {
        userProfile.IsActive = true;
      }

      _context.Entry(userProfile).State = System.Data.EntityState.Modified;
      _context.SaveChanges();
    }

    public void AddUser(UserProfile newUserProfile)
    {
      _context.UserProfiles.Add(newUserProfile);
      _context.SaveChanges(); 
    }

    public void DeleteUser(string mid)
    {
      UserProfile userProfile = _context.UserProfiles.Where(up => up.MID == mid).FirstOrDefault();

      //find all ratings, delete
      DeleteRatingsForUser(userProfile);
      DeleteCommentsForUser(userProfile);
      CheckInBooksForUserOnDeletion(userProfile);
      
      _context.UserProfiles.Remove(userProfile);
      var membership = (SimpleMembershipProvider)Membership.Provider;
      if (Roles.FindUsersInRole(MembershipHelper.ROLE_BASIC_USER,userProfile.MID).Length > 0)
      {
        Roles.RemoveUserFromRole(userProfile.MID, MembershipHelper.ROLE_BASIC_USER); //get rid of the role tie for that id     
      }
      if (Roles.FindUsersInRole(MembershipHelper.ROLE_LIBRARIAN, userProfile.MID).Length > 0)
      {
        Roles.RemoveUserFromRole(userProfile.MID, MembershipHelper.ROLE_LIBRARIAN); //get rid of the role tie for that id     
      }
      if (Roles.FindUsersInRole(MembershipHelper.ROLE_ADMIN, userProfile.MID).Length > 0)
      {
        Roles.RemoveUserFromRole(userProfile.MID, MembershipHelper.ROLE_ADMIN); //get rid of the role tie for that id     
      }
      membership.DeleteAccount(userProfile.MID); //delete account from webpages_Membership table
      Membership.DeleteUser(userProfile.MID, true); // delete user from aspnet_Users table
      
      _context.SaveChanges(); //save changes for remove User Profile from UserProfile table
    }

    public UserProfile Update(UserProfile profile)
    {
      UserProfile userProfileFull = Find(profile.Id);

      userProfileFull.LastName = profile.LastName;
      userProfileFull.FirstName = profile.FirstName;
      userProfileFull.Email = profile.Email;
      userProfileFull.Preferences = profile.Preferences;

      _context.Entry<UserProfile>(userProfileFull).State = System.Data.EntityState.Modified;
      _context.SaveChanges();

      return userProfileFull;
    }

    public List<string> FindlLibrarianAdminMIDList()
    {
      List<string> adminLibrarianMIDList = new List<string>();
      foreach (UserProfile user in _context.UserProfiles)
	    {
        if (Roles.IsUserInRole(user.MID,MembershipHelper.ROLE_LIBRARIAN) || Roles.IsUserInRole(user.MID,MembershipHelper.ROLE_ADMIN))
          adminLibrarianMIDList.Add(user.MID);
	    }

      return adminLibrarianMIDList;
    }

    private void DeleteCommentsForUser(UserProfile profile)
    {
      var commentsForUser = _context.Comments.Where(x => x.UserMID == profile.MID);
      foreach (var comment in commentsForUser)
      {
        _context.Entry(comment).State = System.Data.EntityState.Deleted;
      }
      _context.SaveChanges();
    }

    private void CheckInBooksForUserOnDeletion(UserProfile profile)
    {
      using (var repositories = new ScopedCompositeRepository())
      {
        repositories.Repositories.CheckoutRepository.CheckInBooksForUserOnDeletion(profile);
      }
    }

    private void DeleteRatingsForUser(UserProfile profile)
    {
      var ratings = _context.Ratings.Where(x => x.UserMID == profile.MID);
      foreach (var rating in ratings)
      {
        _context.Entry(rating).State = System.Data.EntityState.Deleted;
      }
      _context.SaveChanges();
    }
    public void ToggleLibrarianPermission(string mid)
    {
      if (Roles.RoleExists(MembershipHelper.ROLE_LIBRARIAN))
      {
        UserProfile userToPromote = FindProfileByMID(mid);
        if (userToPromote != null)
        {
          // Demote
          if (Roles.IsUserInRole(userToPromote.MID,MembershipHelper.ROLE_LIBRARIAN))
          {
            Roles.RemoveUserFromRole(userToPromote.MID, MembershipHelper.ROLE_LIBRARIAN);
          }
          else //Promote
          Roles.AddUserToRole(userToPromote.MID, MembershipHelper.ROLE_LIBRARIAN);
        }
      }
    }

    public List<string> AllUserMids()
    {
      var query = _context.UserProfiles.ToList().ConvertAll(x=>x.MID);
      if (query.Any())
      {
        return query;
      }
      else
      {
        return new List<string>();
      }
    }
    
    public void Dispose()
    {
      // Intentionally empty.
    }
  }

  public interface IUserProfileRepository : IDisposable
  {
    IEnumerable<UserProfile> All { get; }
    IEnumerable<UserProfile> AllIncluding(Expression<Func<UserProfile, bool>> filter = null, string includeProperties = "");
    UserProfile Find(int id);
    UserProfile FindProfileByMID(string mid);
    bool MIDExists(string mid);
    bool MIDIsActive(string mid);
    UserProfile Update(UserProfile profile);
    List<string> FindlLibrarianAdminMIDList();
    List<string> AllUserMids();
    void AddUser(UserProfile newUserProfile);
    void ToggleLibrarianPermission(string mid);
    void ToggleUserActiveState(string mid);
    void DeleteUser(string mid);
	}
}

