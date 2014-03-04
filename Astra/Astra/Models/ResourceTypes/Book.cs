using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using Astra.CustomValidations;

namespace Astra.Models.ResourceTypes
{
  public class Book : Resource
  {
    private string _isbn10;
    private string _isbn13;//new field for isbn 13 digit codes
    private string _isbn;
    private string _author;
    private string _edition;
    private string _publisher;
    private int _pages;

    [StringLength(50, MinimumLength = 0)]
    [Display(Name = "ISBN10 Number")]
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
      }
    }

    [StringLength(50, MinimumLength = 0)]
    [Display(Name = "ISBN13 Number")]
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
      }
    }

    [NotMapped]
    [ISBNValidation]
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
       
    [StringLength(255, MinimumLength = 1)]
    [Required]
    public string Author
    {
      get { return _author; }
      set
      {
        if (!string.IsNullOrWhiteSpace(value))
        { value = value.Trim(); }
        _author = value; 
      }
    }

    [StringLength(50, MinimumLength = 0)]
    public string Edition
    {
      get { return _edition; }
      set 
      {
        if (!string.IsNullOrWhiteSpace(value))
          { value = value.Trim(); }
          _edition = value;
      }
    }

    [StringLength(255, MinimumLength = 0)]
    public string Publisher
    {
      get { return _publisher; }
      set 
      {
        if (!string.IsNullOrWhiteSpace(value))
          { value = value.Trim(); }

          _publisher = value; 
      }
    }

    public int Pages
    {
      get { return _pages; }
      set { _pages = value; }
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
  }
}