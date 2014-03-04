using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Astra.Models.ViewModels
{
  public class ResourceSuggestionPager
  {
    public List<ResourceSuggestion> SuggestionList { get; set; }
    public int PageSize { get; set; }
    public int CurrentPageNumber { get; set; }
    public int TotalSuggestionsInCurrentSet { get; set; }
  }
}