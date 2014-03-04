using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Astra.CompositeRepository;
using PagedList;
using WebMatrix.WebData;
using Astra.Models;
using Astra.Helper;

namespace Astra.Models.ViewModels
{
   
  public class IndexPagerViewModel
  {
    private const int PAGE_SIZE = 5;
    private const int PAGE_SIZE_MAX = 100;
    private const int DEFAULT_PAGE = 1;
    private string _keywords;
    public string Keywords
    {
      get { return _keywords; }
      set { _keywords = value; }
    }

    private BrowsingFrom _currentlyBrowsingFrom;
    public BrowsingFrom CurrentlyBrowsingFrom
    {
      get { return _currentlyBrowsingFrom; }
      set { _currentlyBrowsingFrom = value; }
    }

    private IPagedList<Astra.Models.Resource> _searchResults;
    public IPagedList<Astra.Models.Resource> SearchResults
    {
      get { return _searchResults; }
      set { _searchResults = value; }
    }

    private IPagedList<Astra.Models.Resource> _recentlyAddedItems;
    public IPagedList<Astra.Models.Resource> RecentlyAddedItems
    {
        get { return _recentlyAddedItems; }
        set { _recentlyAddedItems = value; }
    }

    private IPagedList<Astra.Models.Resource> _recentlyViewed;
    public IPagedList<Astra.Models.Resource> RecentlyViewed
    {
        get { return _recentlyViewed; }
        set { _recentlyViewed = value; }
    }

    private IPagedList<Astra.Models.Resource> _recommendations;
    public IPagedList<Astra.Models.Resource> Recommendations
    {
        get { return _recommendations; }
        set { _recommendations = value; }
    }
    
    public enum BrowsingFrom
    {
      Index,
      KeywordSearch,
      AdvancedSearch
    }
    
    [Display(Name = "Results Per Page")]
    public int NumResultsPerPage { get; set; }

    private IEnumerable<SelectListItem> _ddlResultsPerPage;
    public IEnumerable<SelectListItem> DdlResultsPerPage
    {
        get { return _ddlResultsPerPage; }
        set { _ddlResultsPerPage = FillddlResultsPerPage(); }
    }

    public string SortBy { get; set; }
    private IEnumerable<SelectListItem> _ddlSortBy;
    public IEnumerable<SelectListItem> DdlSortBy
    {
        get { return _ddlSortBy; }
        set { _ddlSortBy = FillddlSortOptions(); }
    }

    public IndexPagerViewModel()
    {
      Keywords = string.Empty;
      CurrentlyBrowsingFrom = BrowsingFrom.Index;
      SearchResults = new PagedList<Astra.Models.Resource>(new List<Resource>(), 1, 1);
      Recommendations = new PagedList<Astra.Models.Resource>(new List<Resource>(), 1, 1);
      RecentlyViewed = new PagedList<Astra.Models.Resource>(new List<Resource>(), 1, 1);
      RecentlyAddedItems = new PagedList<Astra.Models.Resource>(new List<Resource>(), 1, 1);
      DdlResultsPerPage = FillddlResultsPerPage();
      DdlSortBy = FillddlSortOptions();
    }

    protected IEnumerable<SelectListItem> FillddlResultsPerPage()
    {
        var ddlResultsOptions = new List<SelectListItem>();
        ddlResultsOptions.Add(new SelectListItem() { Text = "10", Value = "10" });
        ddlResultsOptions.Add(new SelectListItem() { Text = "20", Value = "20" });
        ddlResultsOptions.Add(new SelectListItem() { Text = "30", Value = "30" });
        ddlResultsOptions.Add(new SelectListItem() { Text = "50", Value = "50" });
       
        return ddlResultsOptions;
    }

    protected IEnumerable<SelectListItem> FillddlSortOptions()
    {
        var ddlResultsOptions = new List<SelectListItem>();
        ddlResultsOptions.Add(new SelectListItem() { Selected = false, Text = "Alphabetical", Value = "Alphabetical" });
        ddlResultsOptions.Add(new SelectListItem() { Selected = true, Text = "Avg Rating", Value = "AvgRating" });
        ddlResultsOptions.Add(new SelectListItem() { Selected = false, Text = "Newest", Value = "Newest" });
        ddlResultsOptions.Add(new SelectListItem() { Selected = false, Text = "Most Reviews", Value = "MostReviews" });
        ddlResultsOptions.Add(new SelectListItem() { Selected = false, Text = "Most Relevant", Value = "Relevance" });

        return ddlResultsOptions;
    }

    public IPagedList<Astra.Models.Resource> GetRecentlyViewed(int pageSize)
    {
        IPagedList < Astra.Models.Resource > recentlyViewed = new PagedList<Astra.Models.Resource>(new List<Resource>(), 1, 1);
        // if there's a logged-in user, retrieves recently viewed resources 
        string currentUserName = MembershipHelper.StripOffDomain(WebSecurity.CurrentUserName);
        if (!string.IsNullOrEmpty(currentUserName))
        {
          int userId = WebSecurity.GetUserId(currentUserName);
          CustomProfile customProfile = new CustomProfile(currentUserName);
            using (var composite = new ScopedCompositeRepository())
            {
                var searchResult = composite.Repositories.ResourceRepository.AllCommittedIncluding(null, "CoverImage").ToList();
                var result = searchResult.Where(x => x.ResourceID == 0).ToList();
                foreach (int i in customProfile.RecentlyViewed)
                    foreach (Resource j in searchResult)
                        if (j.ResourceID == i)
                            result.Add(j);
                RecentlyViewed = result.ToPagedList(DEFAULT_PAGE, pageSize);
            }
        }
        return RecentlyViewed;
    }

    public IPagedList<Astra.Models.Resource> GetRecommendations()
    {
        using (var composite = new ScopedCompositeRepository())
        {
            if (!string.IsNullOrEmpty(MembershipHelper.StripOffDomain(WebSecurity.CurrentUserName)))
            {
                var recently = GetRecentlyViewed(PAGE_SIZE_MAX);
                
                List<Resource> r = new List<Resource>();
                foreach (Resource lst in recently)
                    r.Add(lst);
                var recent = AstraUtility.GetRecommendations(r);
                Recommendations = recent.ToPagedList(DEFAULT_PAGE, PAGE_SIZE);
            }

            return Recommendations;
        }
    }

    public IPagedList<Astra.Models.Resource> GetRecentlyAddedItems()
    {
        using (var composite = new ScopedCompositeRepository())
        {
            return AstraUtility.GetRecentlyAddedItems().ToPagedList(DEFAULT_PAGE, PAGE_SIZE_MAX); 
        }
    }

  }
}