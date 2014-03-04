using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Astra.Models.ViewModels
{
  public class KeyWordPercentageDataBank
  {
    #region Constructors
    public KeyWordPercentageDataBank()
    {
      KeyWordPercentageList  = new List<KeyWordPercentage>();
    }

    public KeyWordPercentageDataBank(List<KeyWordPercentage> KeyWordPercentageList)
    {
      this.KeyWordPercentageList = KeyWordPercentageList;
    }
    #endregion

    #region Pubic Data Property
    public List<KeyWordPercentage> KeyWordPercentageList { get; set; }
    #endregion

    #region function methods
    public void CalculateSizesFromAverage(int size1 = 30, int size2 = 20, int size3 = 10, int size4 = 5)
    {

      //find the max number of times any keyword is used
      int max = 0;
      foreach (KeyWordPercentage item in KeyWordPercentageList)
      {
        if (item.NumberOfTimesUsed > max)
          max = item.NumberOfTimesUsed;
      }

      //now calculate the averages
      foreach (KeyWordPercentage item in KeyWordPercentageList)
      {
        item.PercentUsed = (float)item.NumberOfTimesUsed / max; 
      }

      foreach (KeyWordPercentage item in KeyWordPercentageList)
      {
        if (item.PercentUsed > .79)
          item.PixelSize = size1;
        else if (item.PercentUsed > .59)
          item.PixelSize = size2;
        else if (item.PercentUsed > .39)
          item.PixelSize = size3;
        else
          item.PixelSize = size4;
      }
    }

    public void CalculateSizesFromStandardDeviation(int size1 = 30, int size2 = 20, int size3 = 10, int size4 = 5)
    {
      int sum = 0;
      foreach (KeyWordPercentage item in KeyWordPercentageList)
      {
        sum += item.NumberOfTimesUsed;
      }
      double average = (double)sum / KeyWordPercentageList.Count();

      double standardDeviation = 0;
      for (int i = 0; i < KeyWordPercentageList.Count(); i++)
      {
        double x = average - KeyWordPercentageList[i].NumberOfTimesUsed;
        x = x * x; //x squared
        standardDeviation += x;
      }
      standardDeviation = Math.Sqrt(standardDeviation / KeyWordPercentageList.Count());

      foreach (KeyWordPercentage item in KeyWordPercentageList)
      {
        if (item.NumberOfTimesUsed >= (average + standardDeviation))
          item.PixelSize = size1;
        else if ((item.NumberOfTimesUsed >= average) && item.NumberOfTimesUsed < (average + standardDeviation))
          item.PixelSize = size2;
        else if (item.NumberOfTimesUsed >= (average - standardDeviation) && (item.NumberOfTimesUsed < average))
          item.PixelSize = size3;
        else
          item.PixelSize = size4;
      }
    }
    #endregion
  }
}