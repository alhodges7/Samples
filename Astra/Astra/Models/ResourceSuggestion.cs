using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Astra.CustomValidations;

namespace Astra.Models
{
  [Table("ResourceSuggestions")]
  public class ResourceSuggestion : AstraBaseModel
  {
    //make the enumerated Suggestion States a public data field with 
    //field initialization so that it can be accessed in the view
    public enum SuggestionStates { Approved, Rejected, Pending }

    public ResourceSuggestion() : base()
    {
      Status = (int)SuggestionStates.Pending;
    }

    #region Class Properties
    [Key]
    public int ResourceSuggestionId { get; set; }

    [Required]
    [MaxLength(500)]
    public string Title { get; set; }

    [Required]
    [AllowHtml()]
    public string Description
    {
      get {
            if (description != null)
              return description.Replace("&nbsp;", " ");
            else
              return null;
          }

      set { description = value; }
    }
    private string description;

    [Required]
    [StringLength(14, ErrorMessage="Too Large")]
    [RegularExpression(@"^\$(\d{1,3}(\,\d{3})*|(\d+))(\.\d{2})?$|^(\d{1,3}(\,\d{3})*|(\d+))(\.\d{2})?$", ErrorMessage = "Please enter valid US Currency")]
    [Display(Name = "Price")]
    [NotMapped] //displayed in view but not mapped to database
    public string StringPrice
    {
      get { return "$" + Price.ToString(); }

      set { 
            stringPrice = value;
            if (char.IsNumber(stringPrice[0]) == false)
            {
              //trim off currency symbol
              stringPrice = stringPrice.Substring(1); //2nd index to end
            }

            Price = decimal.Parse(stringPrice);
           }
    }
    private string stringPrice;

    public decimal Price { get; set; } //mapped to database

    [Required]
    [Display(Name = "Why should we buy this?")]
    [AllowHtml()]
    public string ReasonNeeded
    {
      get {
            if (reasonNeeded != null)
              return reasonNeeded.Replace("&nbsp;", " ");
            else
              return null;
          }

      set { reasonNeeded = value; }
    }
    private string reasonNeeded;
    

    [Display(Name = "Web URL (If applicable)")]
    public string URL { get; set; }

    [Display(Name = "Librarian's Note (Optional)")]
    [AllowHtml()]
    public string LibrariansNote
    {
      get {
            if (librariansNote != null)
              return librariansNote.Replace("&nbsp;", " ");
            else
              return null;
          }

      set { librariansNote = value; }
    }
    private string librariansNote;

    [Display(Name = "Status")]
    public int Status { get; set; }
    #endregion

    #region Function Methods (and ISBN Properties)
    public string GetStatusString()
    {
      switch (Status)
      {
        case (int)SuggestionStates.Approved:
          return "Approved";

        case (int)SuggestionStates.Pending:
          return "Pending";

        case (int)SuggestionStates.Rejected:
          return "Rejected";

        default:
          throw new Exception("Unknown Suggestion Status integer");
      }
    }

    public override string ToString()
    {
      return "Title: " + this.Title + " Description: " + this.Description + " Price: " + this.StringPrice + " ReasonNeeded: " + this.ReasonNeeded + " LibrariansNote: " + this.LibrariansNote + " URL: " + this.URL + " ISBN: " + this.ISBN + " DateCreated" + this.CreatedOn;
    }

    //private data fields for ISBN numbers
    private string _isbn10; //old 10 digit format
    private string _isbn13; //new 13 digit format
    private string _isbn;   //holds either one

    [StringLength(50, MinimumLength = 0)]
    [ISBNValidator(10)]
    public string ISBN10
    {
      get
      {
        if (!string.IsNullOrEmpty(_isbn10) && _isbn10.Length == 10)
        {
          _isbn10 = FormatISBN(_isbn10);
        }
        return _isbn10;
      }
      set
      {
        _isbn10 = CleanUpISBN(value);
        if (_isbn10 != null)
          _isbn = _isbn10;
      }
    }

    [StringLength(50, MinimumLength = 0)]
    [ISBNValidator(13)]
    public string ISBN13 //new public prop for isbn13 digit codes
    {
      get
      {
        if (!string.IsNullOrEmpty(_isbn13) && _isbn13.Length == 13)
        {
          _isbn13 = FormatISBN(_isbn13);
        }
        return _isbn13;
      }
      set
      {
        _isbn13 = CleanUpISBN(value);
        if (_isbn13 != null)
          _isbn = _isbn13;
      }
    }

    [NotMapped]
    [Display(Name = "ISBN (Optional)")]
    public string ISBN
    {
      get { return _isbn; }
      set
      {
        int count = 0;

        if (value != null)
        {
          value = CleanUpISBN(value);
          _isbn = value;
          count = value.Length;
        }
        if (count == 13)
        {
          _isbn13 = value;
        }
        if (count == 10)
        {
          _isbn10 = value;
        }
      }
    }

    public static string CleanUpISBN(string value)
    {
      if (value != null)
      {
        for (int i = 0; i < value.Length; i++)
        {

          if (!char.IsNumber(value[i]))
          {
            value = value.Remove(i, 1);
            i--;
          }
        }
      }
      return value;
    }

    public static string FormatISBN(string input)
    {
      int numberCount = 0;
      for (int index = 0; index < input.Length; index++)
      {
        if (!char.IsNumber(input[index]))
        {
          input.Remove(index, 1);
        }
        if (char.IsNumber(input[index]))
        {
          numberCount++;
        }
      }
      if (numberCount == 10)
      {
        input = input.Insert(1, "-");
        input = input.Insert(4, "-");
        input = input.Insert(11, "-");
        return input;
      }
      else if (numberCount == 13)
      {
        input = input.Insert(3, "-");
        input = input.Insert(5, "-");
        input = input.Insert(8, "-");
        input = input.Insert(15, "-");
        return input;
      }
      return null;
    }

    public string GetFormattedISBN()
    {
      if (_isbn == null || _isbn == string.Empty)
        return string.Empty;
      else
        return FormatISBN(_isbn);
    }

    //warning: if the description contains rich html content, this function will return half a html tag if it
    //just happens to be at the length limit
    public string GetShortDescription(int length = 120)
    {
      if (Description.Length > length)
        return Description.Substring(0, length) + "...";
      else
        return Description;
    }

    #endregion
  }
}