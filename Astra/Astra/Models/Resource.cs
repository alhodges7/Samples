using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MTUtil;
using MTUtil.Strings;
using Astra.Controllers;
using Astra.CustomValidations;
using MTUtil.TypeManagement;
using WebMatrix.WebData;
using Astra.Repositories;
using Astra.CompositeRepository;

namespace Astra.Models
{

  public enum ResourceSkillLevel : int
  {
    Unknown = 0,
    Novice = 1,
    Intermediate = 2,
    Ninja = 3
  }

  public class Resource : AstraBaseModel
  {
    private int _resourceID;
    private string _title;
    private ResourceType _resourceType;
    private string _description;
    private double _rating;
    private List<Comment> _comments;
    private int? _coverImageId;
    private List<string> _keyWords;
    private bool _committed = false;
    private int _copies = 1;
    private List<ResourceImage> _images;
    private ResourceImage _coverImage;
    private List<ResourceToKeyWordLink> _keyWordLinks = null;
    private List<CheckOut> _checkOutList;
    private float _purchaseCost;
    private DateTime? _purchaseDate;
    private float _replacementCost;
    private float _relevanceAsSearchResult;

    public Resource() : base()
    {
      _relevanceAsSearchResult = 0; //starts at zero, then calculated based on what user searches for
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
      set
      {
        _checkOutList = value;
      }
    }

    [NotMapped]
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

    [Key]
    public int ResourceID
    {
      get { return _resourceID; }
      set { _resourceID = value; }
    }

    //[Required] Bugs adv search
    [StringLength(255, MinimumLength = 2)]
    [Required]
    public string Title
    {
      get { return _title; }
      set 
      {
        if (!string.IsNullOrWhiteSpace(value))
          { value = value.Trim(); }
        _title = value;
      }
    }

    public int? ResourceTypeID { get; set; }

    [Display(Name = "Resource Type")]
    [ForeignKey("ResourceTypeID")]
    public ResourceType ResourceType
    {
      get { return _resourceType; }
      set { _resourceType = value; }
    }

    [StringLength(4000, MinimumLength = 0)]
    public string Description
    {
      get { return _description; }
      set 
      {
        if (!string.IsNullOrWhiteSpace(value))
          { value = value.Trim(); }
        _description = value; 
      }
    }

    [NotMapped]
    public double Rating
    {
      get
      {
        using (var repository = new ScopedCompositeRepository().Repositories.RatingsRepository)
        {
          return repository.GetAverageRatingForResource(_resourceID);
        }
      }
      set
      {
        _rating = value;
      }
    }

    public List<Comment> Comments
    {
      get
      {
        if (_comments == null)
        {
          _comments = new List<Comment>();
        }

        return _comments;
      }
      set { _comments = value; }
    }

    public bool Committed
    {
      get { return _committed; }
      set { _committed = value; }
    }

    public virtual List<ResourceImage> Images
    {
      get
      {
        if (_images == null)
        {
          _images = new List<ResourceImage>();
        }
        return _images;
      }
      set
      {
        _images = value;
      }
    }

    [ForeignKey("CoverImage")]
    public int? CoverImageId
    {
      get { return _coverImageId; }
      set { _coverImageId = value; }
    }

    public ResourceImage CoverImage
    {
      get { return _coverImage; }
      set { _coverImage = value; }
    }

    public int AvailableCopies
    {
      get
      {
        using (var repository = new ScopedCompositeRepository())
        {
          return _copies - repository.Repositories.CheckoutRepository.GetActiveCheckOutsForResource(this.ResourceID).Count();
        }
      }
    }

    [Display(Name = "Librarian's Note (Optional)")]
    public string AdminNote { get; set; }

    [NotMapped]
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

    [NotMapped]
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

    public int Copies
    {
      get { return _copies; }
      set { _copies = value; }
    }

    //Type Discriminator
    [NotMapped]
    public string Discriminator
    {
      get
      {
        if (this.GetType().Name.Contains('_'))
        {
          return this.GetType().Name.Substring(0, this.GetType().Name.IndexOf('_'));
        }
        else
        {
          return this.GetType().Name;
        }
      }
    }

    [Display(Name = "Purchase Cost")]
    public float PurchaseCost
    {
      get { return _purchaseCost; }
      set { _purchaseCost = value; }
    }

    [Display(Name = "Purchase Date")]
    public DateTime? PurchaseDate
    {
      get { return _purchaseDate; }
      set { _purchaseDate = value; }
    }

    [Display(Name = "Replacement Cost")]
    public float ReplacementCost {
      get { return _replacementCost;}
      set { _replacementCost = value; }
    }

    [NotMapped]
    public float RelevanceAsSearchResult
    {
      get { return _relevanceAsSearchResult; }
    }

    public void CalculateRelevanceAsSearchResult(List<string> keywords)
    {
      int keywordCount = keywords.Count();
      int hits = 0;
      foreach (string word in keywords)
      {
        if (this.Title.ToLower().Contains(word.ToLower()))
        {
          hits++;
        }
        else
        {
          List<string> keyWords = this.KeyWords;
          foreach (string officialKeyWord in keyWords)
          {
            if (officialKeyWord.ToLower().Contains(word.ToLower()))
              hits++;
          }
        }
      }

      this._relevanceAsSearchResult = (float)hits / keywordCount;
    }

    public void AddImage(ResourceImage image)
    {
      this.Images.Add(image);
    }
  }
}