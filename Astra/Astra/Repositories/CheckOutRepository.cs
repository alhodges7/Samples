using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Security;
using System.Configuration;
using System.Web.Mvc;
using System.Web.Routing;
using WebMatrix.WebData;
using Astra.Models;
using Astra.Helper;
using Astra.DatabaseContext;
using Astra.CompositeRepository;
using Astra.AstraConfigurations;
using Astra.AstraConfigurations.Settings;

namespace Astra.Repositories
{
  public class CheckOutRepository : ICheckOutRepository
  {
    private AstraContext _context = null;

    public CheckOutRepository(AstraContext context)
    {
      _context = context;
    }

    public IEnumerable<CheckOut> All
    {
      get { return _context.CheckOuts; }
    }

    public IEnumerable<CheckOut> AllIncluding(Expression<Func<CheckOut, bool>> filter = null, string includeProperties = "")
    {
      IQueryable<CheckOut> query = _context.CheckOuts;

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

    public CheckOut Find(int id)
    {
      return _context.CheckOuts.Find(id);
    }

    public List<CheckOut> GetCheckOutsForUser()
    {
      var checkouts = _context.CheckOuts.Where(c => c.UserMID.ToUpper() == MembershipHelper.CurrentUserName().ToUpper()).Include("Resource");
      return checkouts.ToList();
    }

    public List<CheckOut> GetActiveCheckOutsForResource(int resourceId)
    {
      var checkouts = _context.CheckOuts.Where(c => c.ResourceID == resourceId && c.CheckOutStatus == CheckOutStatus.CheckedOut).OrderByDescending(c => c.DateCheckedOut);
      return checkouts.ToList();
    }

    public List<CheckOut> GetActiveCheckOutsForUser(string mid)
    {
      var checkouts = _context.CheckOuts.Where(c => c.UserMID.ToUpper() == mid.ToUpper() && c.CheckOutStatus == CheckOutStatus.CheckedOut).Include(x => x.Resource).OrderByDescending(x => x.DateCheckedOut);
      return checkouts.ToList<CheckOut>();
    }

    public CheckOut FindCheckedOutToUser(Resource resource, string mid)
    {
      return _context.CheckOuts.FirstOrDefault(
        c => c.ResourceID == resource.ResourceID
          && c.CheckOutStatus == CheckOutStatus.CheckedOut
          && c.UserMID.ToUpper() == mid.ToUpper()); //retrieve current resource
    }

    public void InsertOrUpdate(CheckOut checkout)
    {
      if (checkout.CheckOutID == default(int))
      {
        // New entity
        checkout.LastModifiedOn = DateTime.UtcNow;
        _context.CheckOuts.Add(checkout);
      }
      else
      {
        // Existing entity
        checkout.LastModifiedOn = DateTime.UtcNow;
        _context.Entry(checkout).State = EntityState.Modified;
      }
    }

    public bool IsCheckedOutByUser(string mid, Resource model)
    {
      foreach (var item in GetActiveCheckOutsForResource(model.ResourceID))
      {
        if (item.UserMID == mid && item.DateCheckedIn == MTUtil.DateTimes.DateTimeUtils.NullDate)
        {
          return true;
        }
      }
      return false;
    }

    public void Delete(int id)
    {
      var checkout = _context.CheckOuts.Find(id);
      _context.CheckOuts.Remove(checkout);
    }

    public void Save()
    {
      _context.SaveChanges();
    }

    public void Dispose()
    {
      _context.Dispose();
    }

    public List<CheckOut> GetCheckOutHistoryForResource(int resourceId)
    {
      var checkOutHistory = _context.CheckOuts.Include("Resource").Where(x => x.ResourceID == resourceId).OrderBy(x => x.DateCheckedOut).OrderByDescending(x => x.CheckOutStatus != CheckOutStatus.CheckedIn).ToList();

      return checkOutHistory;
    }

    public List<CheckOut> GetCheckOutHistoryForUser(string mid)
    {
      var checkOutHistory = _context.CheckOuts.Where(x => x.UserMID.ToUpper() == mid.ToUpper()).OrderByDescending(x => x.DateCheckedOut).OrderByDescending(x => x.CheckOutStatus != CheckOutStatus.CheckedIn).ThenByDescending(x => x.DateCheckedOut).Include("Resource").ToList();

      return checkOutHistory;
    }

    public void CheckInBooksForUserOnDeletion(UserProfile profile)
    {
      var checkOutsForUser = GetActiveCheckOutsForUser(profile.MID);
      foreach (var checkout in checkOutsForUser)
      {
        checkout.CheckOutStatus = CheckOutStatus.CheckedIn;
        checkout.DateCheckedIn = DateTime.UtcNow;
        _context.Entry(checkout).State = System.Data.EntityState.Modified;
      }
      _context.SaveChanges();
    }

    public List<UsersListViewModel> FiveMostRecentCheckOutsForResource(Resource model)
    {
      List<UsersListViewModel> checkedOutToList = new List<UsersListViewModel>();
      var users = (from c in _context.CheckOuts
                   join u in _context.UserProfiles on c.UserMID equals u.MID
                   where model.ResourceID == c.ResourceID
                   && c.CheckOutStatus == CheckOutStatus.CheckedOut
                   orderby c.DateCheckedOut descending
                   select new UsersListViewModel
                   {
                     MID = u.MID,
                     FirstName = u.FirstName,
                     CheckedInDate = c.DateCheckedIn,
                     CheckedOutDate = c.DateCheckedOut
                   }).Take(5);

      foreach (var user in users)
      {
        // add the users that have not yet checked in their resource
        if (user.CheckedInDate == MTUtil.DateTimes.DateTimeUtils.NullDate)
        {
          checkedOutToList.Add(user);
        }
      }
      return checkedOutToList;
    }

    public List<UsersListViewModel> FiveMostRecentCheckInsForResource(Resource model)
    {
      List<UsersListViewModel> checkedInList = new List<UsersListViewModel>();
      var users = (from c in _context.CheckOuts
                   join u in _context.UserProfiles on c.UserMID equals u.MID
                   where model.ResourceID == c.ResourceID
                   && c.CheckOutStatus == CheckOutStatus.CheckedIn
                   orderby c.DateCheckedIn descending
                   select new UsersListViewModel
                   {
                     MID = u.MID,
                     FirstName = u.FirstName,
                     CheckedInDate = c.DateCheckedIn,
                     CheckedOutDate = c.DateCheckedOut
                   }).Take(5);

      foreach (var user in users)
      {
        if (user.CheckedInDate != MTUtil.DateTimes.DateTimeUtils.NullDate)
          checkedInList.Add(user);
      }

      return checkedInList;
    }

    public List<ISummaryHistoryItem> RecentResourceActivity(int resourceId)
    {
      // most recent 5 check outs
      var recentChkOutHistory = (from chk in _context.CheckOuts
                                 where chk.ResourceID == resourceId
                                 orderby chk.DateCheckedOut descending
                                 select chk).Take(5).ToList();
      // most recent 5 check ins
      var recentChkInHistory = (from chk in _context.CheckOuts
                                where chk.ResourceID == resourceId && chk.CheckOutStatus == CheckOutStatus.CheckedIn
                                orderby chk.DateCheckedIn descending
                                select chk).Take(5).ToList();

      var recentReservationHistory = (from reservation in _context.Reservations
                                      where reservation.ResourceID == resourceId
                                      orderby reservation.DateReserved descending
                                      select reservation).Take(5).ToList();

      var recentCancelledReservationHistory = (from reservation in _context.Reservations
                                               where reservation.ResourceID == resourceId && reservation.ReservationStatus == ReservationStatus.Cancelled
                                               orderby reservation.DateReserved descending
                                               select reservation).Take(5).ToList();

      List<ISummaryHistoryItem> historyList = new List<ISummaryHistoryItem>();

      foreach (var item in recentChkOutHistory)
      {
        historyList.Add(GetHistoryItemForCheckout(item));
      }
      foreach (var item in recentChkInHistory)
      {
        historyList.Add(GetHistoryItemForCheckin(item));
      }
      foreach (var item in recentReservationHistory)
      {
        historyList.Add(GetHistoryItemForReservation(item));
      }
      foreach (var item in recentCancelledReservationHistory)
      {
        historyList.Add(GetHistoryItemForCancelledReservation(item));
      }

      historyList.Sort();
      historyList.Reverse();

      return historyList.GetRange(0, historyList.Count() < 5 ? historyList.Count() : 5);
    }

    private ISummaryHistoryItem GetHistoryItemForCheckout(CheckOut checkout)
    {
      using (var repository = new ScopedCompositeRepository().Repositories.UserProfileRepository)
      {
        UserProfile userProfileObj = repository.FindProfileByMID(checkout.UserMID);
        return new SummaryCheckOutHistoryItem(checkout.DateCheckedOut, "Checked out for ", userProfileObj.FirstName + " " + userProfileObj.LastName, checkout.CreatedByMID);
      }
    }

    private ISummaryHistoryItem GetHistoryItemForCheckin(CheckOut checkout)
    {
      using (var repository = new ScopedCompositeRepository().Repositories.UserProfileRepository)
      {
        UserProfile userProfileObj = repository.FindProfileByMID(checkout.UserMID);
        return new SummaryCheckOutHistoryItem(checkout.DateCheckedIn, "Checked in for ", userProfileObj.FirstName + " " + userProfileObj.LastName, checkout.LastModifiedByMID);
      }
    }

    private ISummaryHistoryItem GetHistoryItemForReservation(Reservation reservation)
    {
      using (var repository = new ScopedCompositeRepository().Repositories.UserProfileRepository)
      {
        UserProfile userProfileObj = repository.FindProfileByMID(reservation.UserMID);
        return new SummaryReservationHistoryItem(reservation.DateReserved, string.Format("Reserved by {0} {1}", userProfileObj.FirstName, userProfileObj.LastName));
      }
    }

    private ISummaryHistoryItem GetHistoryItemForCancelledReservation(Reservation reservation)
    {
      using (var repository = new ScopedCompositeRepository().Repositories.UserProfileRepository)
      {
        UserProfile userProfileObj = repository.FindProfileByMID(reservation.UserMID);
        return new SummaryReservationHistoryItem(reservation.LastModifiedOn.ToLocalTime(), string.Format("Reservation cancelled by {0} {1}", userProfileObj.FirstName, userProfileObj.LastName));
      }
    }

    public void CheckIn(int resourceId, string username, bool userIsAdmin)
    {
      List<CheckOut> activeChkOutsList = this.GetActiveCheckOutsForResource(resourceId);
      CheckOut checkinCandidate = activeChkOutsList.Where(x => x.UserMID.ToUpper() == username.ToUpper() && x.ResourceID == resourceId).FirstOrDefault();

      if (checkinCandidate == null)
      {
        throw new Exception("Could not locate valid checkin candidate.");
      }

      checkinCandidate.DateCheckedIn = DateTime.UtcNow;
      checkinCandidate.CheckOutStatus = CheckOutStatus.CheckedIn;

      _context.Entry(checkinCandidate).State = EntityState.Modified;

      _context.SaveChanges();

      using (var repository = new ScopedCompositeRepository().Repositories)
      {
        Reservation target = repository.ReservationRepository.AcceptOldestReservation(resourceId);

        if (target != null)
        {
          repository.CheckoutRepository.CheckOut(resourceId, target.UserMID, MembershipHelper.CurrentUserIsAdminOrLibrarian());

          repository.UserMessageRepository.SendSystemMessage(BuildReservationNotification(resourceId), target.UserMID, null);
        }
      }
    }

    private string BuildReservationNotification(int resourceId)
    {
      using (var repository = new ScopedCompositeRepository().Repositories)
      {
        var resource = repository.ResourceRepository.Find<Resource>(resourceId);
        RouteValueDictionary routeValues = new RouteValueDictionary();
        routeValues.Add("resourceId", resource.ResourceID);
        RequestContext requestContext = HttpContext.Current.Request.RequestContext;
        
        return string.Format("The resource \"{0}\" has automatically been checked out to you based on your reservation. Don't forget to pick up your resource! If you are no longer interested in this resource, please click {1} and check in to make the resource available to other users.", resource.Title, HtmlHelper.GenerateLink(requestContext, RouteTable.Routes, "here", "Default", "Summary", "Resources", routeValues, null));
      }
    }

    public bool CheckOut(int resourceId, string username, bool callerIsAdmin)
    {
      Resource resource = null;
      List<CheckOut> checkOutList = null;

      using (var repositories = new ScopedCompositeRepository().Repositories)
      {
        resource = repositories.ResourceRepository.Find<Resource>(resourceId);
        checkOutList = repositories.CheckoutRepository.GetActiveCheckOutsForResource(resourceId);
      }

      CheckOut checkOut = new CheckOut();
      checkOut.ResourceID = resourceId;
      checkOut.UserMID = username;
      checkOut.CheckOutStatus = CheckOutStatus.CheckedOut;

      //check if user already has book checked out
      if (checkOutList.Where(x => x.UserMID.ToUpper() == username.ToUpper()).Any())
      {
        throw new Exception("User has resource checked out already.");
      }

      if (resource.Copies > checkOutList.Count)
      {
        this.InsertOrUpdate(checkOut);
        this.Save();
      }

      return true;
    }
    public List<CheckOut> OverdueBooks()
    {

      OverdueBookNotificationSection _configuration = AstraConfigurationManager.OverdueBookSettings();
      //earliest date when the resource wont be overdue
      //any date before that will make resource overdue
      //any date after that will make resource not overdue
      DateTime overdueDate = DateTime.UtcNow.Subtract(TimeSpan.FromHours(_configuration.OverdueBookDeadline));
      List<CheckOut> results = _context.CheckOuts.Where(
                                                        x => x.CheckOutStatus == CheckOutStatus.CheckedOut
                                                          &&
                                                        x.DateCheckedOut >= overdueDate
                                                        ).OrderBy(mid => mid.UserMID).Include("Resource").ToList();

      _configuration.LastOverdueCheck = DateTime.UtcNow.ToString();
      // save will be auto now
      if (results == null)
      {
        return results = new List<CheckOut>();
      }
      return results;
    }

    public System.Web.Routing.RequestContext CheckOutController { get; set; }
  }

  public interface ICheckOutRepository : IDisposable
  {
    IEnumerable<CheckOut> All { get; }
    IEnumerable<CheckOut> AllIncluding(Expression<Func<CheckOut, bool>> filter = null, string includeProperties = "");
    List<CheckOut> GetCheckOutsForUser();
    List<CheckOut> GetActiveCheckOutsForResource(int resourceId);
    List<CheckOut> GetActiveCheckOutsForUser(string mid);
    List<CheckOut> OverdueBooks();
    CheckOut FindCheckedOutToUser(Resource resource, string mid);
    bool IsCheckedOutByUser(string mid, Resource model);
    CheckOut Find(int id);
    void CheckIn(int resourceId, string username, bool userIsAdmin);
    bool CheckOut(int resourceId, string username, bool userIsAdmin);
    void InsertOrUpdate(CheckOut checkout);
    void Delete(int id);
    void Save();
    List<CheckOut> GetCheckOutHistoryForResource(int resourceId);
    List<CheckOut> GetCheckOutHistoryForUser(string mid);
    void CheckInBooksForUserOnDeletion(UserProfile profile);
    List<ISummaryHistoryItem> RecentResourceActivity(int recourceId);
  }
}