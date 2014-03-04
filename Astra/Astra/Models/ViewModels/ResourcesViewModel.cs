using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using Astra.CustomValidations;
using Astra.CompositeRepository;
using Astra.Models.ResourceTypes;


namespace Astra.Models.ViewModels
{
  public class ResourcesViewModel
  {
    private int _resourceID;
    private string _title;
    private ResourceType _resourceType;
    private string _author;
    private double _rating;
    private string _isbn10;
    private string _isbn13;
    private string _isbn;
    private int _copies = 1;
    private int _checkedOutCopies;
    private List<CheckOut> _checkOutList;
    private int _skillLevel;
    private List<string> _keyWords;
    private List<ResourceToKeyWordLink> _keyWordLinks = null;
    public ResourcesViewModel()
    {
      
      // intentionally empty
    }

    public ResourcesViewModel(Book input)
    {
      this._resourceID = input.ResourceID;
      this._author = input.Author;
      this._title = input.Title;
      this._rating = input.Rating;
      this._isbn10 = input.ISBN10;
      this._isbn13 = input.ISBN13;
      this._isbn = input.ISBN;
      this._copies = input.Copies;
      this._checkedOutCopies = 0;
      this._checkOutList = input.CheckOutList;
    }

    public int ResourceID
    {
      get { return _resourceID; }
      set { _resourceID = value; }
    }

    public string Title
    {
      get { return _title; }
      set { _title = value; }
    }

    public string Author
    {
      get { return _author; }
      set { _author = value; }
    }

    public double Rating
    {
      get { return _rating; }
      set { _rating = value; }
    }

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

    public int Copies
    {
      get { return _copies; }
      set { _copies = value; }
    }


    public virtual List<CheckOut> CheckOutList
    {
      get
      {
        if (_checkOutList == null)
        {
          _checkOutList = new List<CheckOut>();
        }
        return _checkOutList;
      }
      set { _checkOutList = value; }
    }

    public int CheckedOutCopies
    {
      get { return _checkedOutCopies; }
      set
      {
        _checkedOutCopies = Copies - CheckOutList.Where(x => x.CheckOutStatus == Astra.Models.CheckOutStatus.CheckedOut || x.CheckOutStatus == Astra.Models.CheckOutStatus.Lost).Count();
      }
    }

    public bool IsAvailable
    {
      get
      {
        if (this.CheckOutList.Where(x => x.CheckOutStatus == CheckOutStatus.CheckedOut).Count() < this.Copies)
        {
          return true;
        }
        return false;
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

    public List<ResourceToKeyWordLink> KeyWordLinks
    {
      get
      {
        if (_keyWordLinks == null)
        {
          using (var repository = new ScopedCompositeRepository())
          {
            _keyWordLinks = repository.Repositories.MiscRepository.GetResourceToKeyWordLinksForResource(this.ResourceID).ToList();
          }
        }
        return _keyWordLinks;
      }
      set { _keyWordLinks = value; }
    }

    [Display(Name = "Key Words")]
    public List<string> KeyWords
    {
      get
      {
        if (_keyWords == null)
        {
          _keyWords = new List<string>();
          using (var repository = new ScopedCompositeRepository().Repositories.KeyWordLinksRepository)
          {
            _keyWords = new List<string>();
            foreach (ResourceToKeyWordLink keyWordLink in this.KeyWordLinks)
            {
              ResourceToKeyWordLink fullLoadedLink = repository.GetFullyLoadedResourceLink(keyWordLink.LinkID);
              _keyWords.Add(fullLoadedLink.KeyWord.Word);
            }
          }
        }
        return _keyWords;
      }
      set
      {
        _keyWords = value;
      }
    }

    public ResourceType ResourceType
    {
      get { return _resourceType; }
      set { _resourceType = value; }
    }

    public int? ResourceTypeID { get; set; }

    public int SkillLevel
    {
      get { return _skillLevel; }
      set { _skillLevel = value; }
    }

    public void Clear()
    {
      _resourceID = 0;
      _title = null;
      _rating = 0;
      _isbn = null;
      _isbn10 = null;
      _isbn13 = null;
      _copies = 1;
      _checkedOutCopies = 0;
      _checkOutList = null;
    }

    public MultiSelectList KeyWordMultiSelectList(List<KeyWord> selectedWords, List<KeyWord> AllKeywords)
    {
      List<SelectListItem> items = new List<SelectListItem>();
      foreach (KeyWord keyword in AllKeywords)
      {
        SelectListItem item = new SelectListItem();
        item.Text = keyword.Word;
        item.Value = keyword.Word;
        item.Selected = true;
        items.Add(item);
      }

      List<string> selectedItems = new List<string>();
      foreach (KeyWord keyword in selectedWords)
      {
        selectedItems.Add(keyword.Word);
      }

      MultiSelectList m = new MultiSelectList(items, "Value", "Text", selectedItems);

      return m;
    }
  }
}
