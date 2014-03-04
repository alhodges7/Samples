using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using WebMatrix.WebData;
using Astra.Controllers.Shared;
using Astra.Misc;
using Astra.DatabaseContext;
using Astra.Models;
using Astra.Models.ResourceTypes;
using Astra.Models.ViewModels;
using Astra.Repositories;
using MTUtil.Strings;
using MTUtil.TypeManagement;
using PagedList;
using Astra.CompositeRepository;
using Astra.Helper;
using Astra.Filters;

namespace Astra.Controllers
{
  public class ResourcesController : BaseController
  {
    private const int RESULTS_PER_PAGE = 10;
    private const string RELEVANCE = "Relevance";
    private const string ALPHABETICAL = "Alphabetical";
    private const string AVG_RATING = "AvgRating";
    private const string NEWEST = "Newest";
    private const string MOST_REVIEWS = "MostReviews";
    private const int PAGE_SIZE = 5;

    //emtpy default constructor
    public ResourcesController() : base()
    {
    }

    [NoCache]
    [HttpGet]
    [Audit]
    public ViewResult Index(int page = 1, int NumResultsPerPage = 10, string sortBy = ALPHABETICAL)
    {
      ViewBag.Title = "Browse Resources";

      //for deletion
      if (TempData["success"] != null)
      {
        ViewBag.Message = TempData["success"];
      }
      
      SaveUserPreferences(string.Empty, NumResultsPerPage, sortBy, 0);

      var SearchResults = GetSortedResults(null, null, sortBy);
      IndexPagerViewModel view = new IndexPagerViewModel();
      view.CurrentlyBrowsingFrom = IndexPagerViewModel.BrowsingFrom.Index;
      view.SearchResults = SearchResults.ToPagedList(page, NumResultsPerPage);
      view.SortBy = sortBy;
      view.NumResultsPerPage = NumResultsPerPage;
      view.RecentlyViewed = view.GetRecentlyViewed(PAGE_SIZE);
      return View(view);
    }

    public ViewResult Details(int id)
    {
      using (var repository = new ScopedCompositeRepository())
      {
        return View(repository.Repositories.ResourceRepository.Find<Resource>(id));
      }
    }

    [NoCache]
    [Audit(AuditingLevel = 2)]
    public ActionResult Summary(int resourceId)
    {
      SummaryViewModel viewModel = new SummaryViewModel();
      // See if the User already has the resource checked out 
      //  (and, if not, can he check it out?)
      using (var repository = new ScopedCompositeRepository().Repositories)
      {
        ViewBag.RecentActivity = repository.CheckoutRepository.RecentResourceActivity(resourceId);
        ViewBag.CurrentCheckouts = repository.CheckoutRepository.GetActiveCheckOutsForResource(resourceId);
        ViewBag.CurrentReservations = repository.ReservationRepository.GetActiveReservationsForResource(resourceId);
        viewModel.UserHasResourceReserved = repository.ReservationRepository.DoesUserHaveResourceReserved(MembershipHelper.CurrentUserName(), resourceId);
        ViewBag.UserHasResourceReserved = repository.ReservationRepository.DoesUserHaveResourceReserved(MembershipHelper.CurrentUserName(), resourceId);
        viewModel.Resource = repository.ResourceRepository.AllCommittedIncluding(x => x.ResourceID == resourceId, "CoverImage,Comments,Images").FirstOrDefault();
        ViewBag.UserHasResourceCheckedOut = repository.CheckoutRepository.IsCheckedOutByUser(MembershipHelper.CurrentUserName(), viewModel.Resource);
        List<CheckOut> checkOutsForAllUsers = repository.CheckoutRepository.GetActiveCheckOutsForResource(viewModel.Resource.ResourceID);

        viewModel.UserHasResourceCheckedOut = repository.CheckoutRepository.IsCheckedOutByUser(MembershipHelper.CurrentUserName(), viewModel.Resource);
        if (viewModel.UserHasResourceCheckedOut)
        {
          CheckOut checkOutForCurrentResource = repository.CheckoutRepository.FindCheckedOutToUser(viewModel.Resource, MembershipHelper.CurrentUserName());
          viewModel.UserCheckedOutResourceDate = checkOutForCurrentResource.DateCheckedOut;
        }
        else
        {
          viewModel.UserHasResourceReserved = repository.ReservationRepository.DoesUserHaveResourceReserved(MembershipHelper.CurrentUserName(), resourceId);
        }

        viewModel.CopiesAvailable = viewModel.Resource.Copies - checkOutsForAllUsers.Count;
        //check if user has commented
        Comment userComment = repository.CommentRepository.GetUserCommentForResource(resourceId, MembershipHelper.CurrentUserName());
        if (userComment != null)
        {
          viewModel.Comment = userComment;
        }
        else
        {
          userComment = new Comment();
          viewModel.Comment = userComment;
        }
      }

      SaveUserPreferences(string.Empty, 0, string.Empty, viewModel.Resource.ResourceID);

      string partialToRender = null;
      switch (viewModel.Resource.Discriminator)
      {
        case "Book":
          partialToRender = "ResourceTypes/_SummaryDetailsBook";
          break;
        case "DVD":
          partialToRender = "ResourceTypes/_SummaryDetailsDVD";
          break;
        case "EBook":
          partialToRender = "ResourceTypes/_SummaryDetailsEBook";
          break;
        case "Hardware":
          partialToRender = "ResourceTypes/_SummaryDetailsHardware";
          break;
        case "Software":
          partialToRender = "ResourceTypes/_SummaryDetailsSoftware";
          break;
      }
      ViewBag.ResourcetypeSpecificDetails = partialToRender;
      return View(viewModel);
    }

    public Resource GetFullLoadedResource(int resourceID)
    {
      using (var repository = new ScopedCompositeRepository())
      {
        return repository.Repositories.ResourceRepository.All.First(r => r.ResourceID == resourceID);
      }
    }

    public string SkillLevelDescription(int skillLevelVal)
    {
      ResourceSkillLevel skillLevel = TypeUtils.ToEnum<ResourceSkillLevel>(skillLevelVal);
      return skillLevel.ToString();
    }

    #region ImageHandling

    [NoCache]
    public ActionResult GetImageList(int resourceId = 0)
    {
      Resource targetResource = null;

      using (var repository = new ScopedCompositeRepository())
      {
        targetResource = repository.Repositories.ResourceRepository.AllIncluding(x => x.ResourceID == resourceId, "Images").FirstOrDefault();
      }

      ViewData["isReadOnly"] = false;

      return PartialView("_ImageList", targetResource);
    }

    [NoCache]
    public ActionResult GetCoverImage(int resourceId = 0)
    {
      Resource targetResource = null;

      using (var repository = new ScopedCompositeRepository())
      {
        targetResource = repository.Repositories.ResourceRepository.AllIncluding(x => x.ResourceID == resourceId, "CoverImage").FirstOrDefault();
      }

      ViewData["isReadOnly"] = false;

      return PartialView("_SingleCreateEditCoverImage", new ResourceImageViewModel() { ResourceId = targetResource.ResourceID, Image = targetResource.CoverImage });
    }

    [NoCache]
    public ActionResult GetCoverImageId(int resourceId = 0)
    {
      Resource targetResource = null;

      using (var repository = new ScopedCompositeRepository())
      {
        targetResource = repository.Repositories.ResourceRepository.AllIncluding(x => x.ResourceID == resourceId, "CoverImage").FirstOrDefault();
      }

      int id = 0;
      if (targetResource != null && targetResource.CoverImageId.HasValue)
      {
        id = targetResource.CoverImageId.Value;
      }

      return Json(
        new
        {
          ID = id
        },
        JsonRequestBehavior.AllowGet
      );
    }
    #endregion

    #region SearchHandling

    public ViewResult QuickSearch(string keyWords, int pageNumber = 1, int NumResultsPerPage = 10, string sortBy = RELEVANCE)
    {
      string initialKeywordSearch = keyWords;

      keyWords = EscapeEmbeddedSpaces(keyWords);
      keyWords = keyWords.Replace("\"", ",");
      keyWords = keyWords.Replace(" ", ",");
      keyWords = keyWords.Replace(";", ",");

      List<string> keyWordsLst = StringUtils.SplitToList(keyWords, ",", StringSplitOptions.RemoveEmptyEntries);
      for (int i = 0; i < keyWordsLst.Count; i++)
      {
        keyWordsLst[i] = keyWordsLst[i].Replace("%20%", " ");
        keyWordsLst[i] = keyWordsLst[i].Replace("'", "''");
      }

      SaveUserPreferences(keyWords, NumResultsPerPage, sortBy, 0);

      var searchResults = GetSortedResults(null, keyWordsLst, sortBy);
      IndexPagerViewModel view = new IndexPagerViewModel();
      view.CurrentlyBrowsingFrom = IndexPagerViewModel.BrowsingFrom.KeywordSearch;
      view.SearchResults = searchResults.ToPagedList(pageNumber, NumResultsPerPage);
      view.SortBy = sortBy;
      view.NumResultsPerPage = NumResultsPerPage;
      view.RecentlyViewed = view.GetRecentlyViewed(PAGE_SIZE);
      view.Keywords = initialKeywordSearch;
      return View("Index", view);
    }

    [HttpGet]
    public ActionResult AdvSearchUsePrevious(string searchTerm = null, int pageNumber = 1, int NumResultsPerPage = 10, string sortBy = "")
    {
      return AdvSearch(TempData["AdvSearchCriteria"] as Resource, searchTerm, pageNumber, NumResultsPerPage, sortBy);
    }

    
    public ActionResult SearchTypes()
    {
      IEnumerable<ResourceType> resourceTypes;
      using (var repository = new ScopedCompositeRepository().Repositories.ResourceTypeRepository)
      {
        resourceTypes = repository.All;
      }
      IEnumerable<SelectListItem> selectList =
     from c in resourceTypes
     select new SelectListItem
     {
       Text = c.Name,
       Value = c.ResourceTypeID.ToString(),

     };
      ViewBag.ResourceTypes = selectList;

      return PartialView("~/Views/Search/_AdvSearch.cshtml");
    }
    [HttpPost]
    [Audit(AuditingLevel = 3)]
    public ActionResult AdvSearch(Resource searchforResource = null, string searchByAuthor = null, int pageNumber = 1, int NumResultsPerPage = 10, string sortBy = "")
    {    
      List<string> keywords = new List<string>();
      List<Resource> searchResult = new List<Resource>();

      if (searchByAuthor != null)
      {
        Book AuthorSearch = new Book();
        AuthorSearch.Author = searchByAuthor;
        searchResult = GetSortedResults(AuthorSearch, null, sortBy);
      }
      else
      {
        searchResult = GetSortedResults(searchforResource, null, sortBy);
      }

      ViewBag.Title = "Search Results";
      TempData["AdvSearchCriteria"] = searchforResource;

      IndexPagerViewModel view = new IndexPagerViewModel();
      view.CurrentlyBrowsingFrom = IndexPagerViewModel.BrowsingFrom.AdvancedSearch;
      view.SearchResults = searchResult.ToPagedList(pageNumber, NumResultsPerPage);
      view.SortBy = sortBy;
      view.NumResultsPerPage = NumResultsPerPage;
      view.RecentlyViewed = view.GetRecentlyViewed(PAGE_SIZE);
      return View("Index", view);
    }

    [HttpGet]
    public ActionResult LoadSearchFields(int resourcetypeID = 1)
    {
      switch (resourcetypeID)
      {
        case 1:
          return PartialView("~/Views/Search/ResourceTypes/_BookSearch.cshtml");
        case 2:
          return PartialView("~/Views/Search/ResourceTypes/_DVDSearch.cshtml");
        case 3:
          return PartialView("~/Views/Search/ResourceTypes/_EBookSearch.cshtml");
        case 4:
          return PartialView("~/Views/Search/ResourceTypes/_HardwareSearch.cshtml");
        case 5:
          return PartialView("~/Views/Search/ResourceTypes/_SoftwareSearch.cshtml");
        case 6:
          return PartialView("~/Views/Search/ResourceTypes/_WhitePaperSearch.cshtml");
        default:
          return null;
      }
    }
    #endregion

    #region Methods
    protected override void Dispose(bool disposing)
    {
      base.Dispose(disposing);
    }

    protected string EscapeEmbeddedSpaces(string test)
    {
      bool openQuote = false;
      string escapedLine = string.Empty;
      for (int x = 0; x < test.Length; x++)
      {
        string oneChar = test.Substring(x, 1);
        if (oneChar == "\"")
          openQuote = !openQuote;
        if (oneChar == " " && openQuote)
          oneChar = "%20%";

        escapedLine += oneChar;
      }
      return escapedLine;
    }

    protected List<Resource> GetSortedResults(Resource searchValues, List<string> keyWords = null, string sortBy = "")
    {
      List<Resource> SearchResults = new List<Resource>();

      if (searchValues != null)
            SearchResults = AstraUtility.DoAdvancedSearch(searchValues);
      else if (keyWords != null && keyWords.Count > 0)
            SearchResults = AstraUtility.DoKeyWordSearch(keyWords);
      else
            SearchResults = AstraUtility.GetAllSearch();

      if (string.Compare(sortBy, ALPHABETICAL) != 0)
        SearchResults = SortListBySortExpr(SearchResults, sortBy);

      return SearchResults;

    }

    protected List<Resource> SortListBySortExpr(List<Resource> list, string sortBy)
    {
      List<Resource> sortedList = new List<Resource>();
      if (list != null && sortBy != string.Empty)
      {
        switch (sortBy)
        {
          case AVG_RATING: var result = list.OrderByDescending(x => x.Rating);
            sortedList = (List<Resource>)result.ToList();
            break;
          case NEWEST: result = list.OrderByDescending(x => x.CreatedOn);
            sortedList = (List<Resource>)result.ToList();
            break;
          case MOST_REVIEWS: result = list.OrderByDescending(x => x.Comments.Count);
            sortedList = (List<Resource>)result.ToList();
            break;
          case RELEVANCE: result = list.OrderByDescending(x => x.RelevanceAsSearchResult);
            sortedList = (List<Resource>)result.ToList();
            break;
            
        }
        return sortedList;
      }

      return list;
    }

    protected bool SaveUserPreferences(string keyWords, int NumResultsPerPage, string sortBy, int ViewedId)
    {
      string currentUserName = MembershipHelper.StripOffDomain(WebSecurity.CurrentUserName);

      if (!string.IsNullOrWhiteSpace(currentUserName))
      {
        int userId = WebSecurity.GetUserId(currentUserName);
        CustomProfile customProfile = new CustomProfile(currentUserName);
        customProfile.DefaultSort = sortBy;
        if (NumResultsPerPage > 0)
          customProfile.NumResultsPerPage = NumResultsPerPage;
        if (keyWords != string.Empty)
          customProfile.KeyWords = keyWords;
        if (ViewedId != 0)
          customProfile.AddToRecentlyViewed(ViewedId);
        customProfile.SaveCustomProfile();
        return true;
      }
      return false;
    }
    #endregion
  }
}

