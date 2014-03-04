using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Astra.Models.ViewModels
{
  public class NewResourcesNotification
  {
    public UserProfile User { get; set; }
    public List<Resource> NewResources { get; set; }
  }
}