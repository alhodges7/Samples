using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Astra.Models.ViewModels
{
  public class KeyWordPercentage: IComparable
  {
    #region Properties
    public string Word { get; set; }
    public int NumberOfTimesUsed { get; set; }
    public float PercentUsed { get; set; }
    public int PixelSize { get; set; }
    #endregion

    #region IComparable implimentation for built in sorting
    public int CompareTo(object obj)
    {
      // If other is not a valid object reference, this instance is greater. 
      if (obj == null) return 1;

      KeyWordPercentage otherKeyWordPercentageObject = obj as KeyWordPercentage;
      return this.Word.CompareTo(otherKeyWordPercentageObject.Word);
    }
    #endregion


    #region Member functions
    public string DisplayWithCalculatedFontSizeStyle()
    {
      return "<span style=\"font-size:" + PixelSize.ToString() + "px;\">" + Word + "</span>";
    }

    public string CalculatedFontSizeStyle()
    {
      return "<span style=\"font-size:" + PixelSize.ToString() + "px;\">";
    }

    public string CalculatedFontSize()
    {
      return PixelSize.ToString() + "px;";
    }

    public string SuggestedFontSize()
    {
      if (PercentUsed > .79)
        return "30px";
      else if (PercentUsed > .59)
        return "20px";
      else if (PercentUsed > .39)
        return "15px";
      else
        return "10px";
    }
    #endregion
  }
}