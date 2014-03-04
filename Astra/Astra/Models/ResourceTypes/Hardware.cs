using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Astra.Models.ResourceTypes
{
  public class Hardware : Resource
  {
    private string _dimensions;

    public string Dimensions
    {
      get { return _dimensions; }
      set { _dimensions = value; }
    }
    private string _modelno;

    public string Modelno
    {
      get { return _modelno; }
      set { _modelno = value; }
    }

  }
}