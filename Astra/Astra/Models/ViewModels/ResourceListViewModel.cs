using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Astra.Models.ViewModels
{
  public class ResourceListViewModel
  {
    public int ResourceId { get; set; }
    public int NumberOfCopies { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
  }
}