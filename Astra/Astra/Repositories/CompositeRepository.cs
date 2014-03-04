using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using Astra.DatabaseContext;
using Astra.Models;
using Astra.Repositories;

namespace Astra.CompositeRepository
{
  public class CompositeRepository : IDisposable
  {
    #region Fields

    private AstraContext _context = null;
    private List<IDisposable> _disposeList = null;
    private bool _disposed = false;

    #endregion

    #region Constructors

    public CompositeRepository()
    {
      _context = new AstraContext();
      _context.Configuration.LazyLoadingEnabled = false;
      _disposeList = new List<IDisposable>();
    }

    #endregion

    #region Properties

    // Try not to use property, it's provided for special cases but virtually functionality should be 
    //   routed through repositories!
    public AstraContext Context
    {
      get
      {
        return _context;
      }
    }

    private IResourceRepository _resourceRepository = null;
    public IResourceRepository ResourceRepository
    {
      get
      {
        if (_resourceRepository == null)
        {
          IUnityContainer container = new UnityContainer();
          container.RegisterType<IAstraContext, AstraContext>();
          container.RegisterType<IResourceRepository, ResourceRepository>();
          _resourceRepository = container.Resolve<IResourceRepository>();

          _disposeList.Add(_resourceRepository);
        }
        return _resourceRepository;
      }
    }

    private ICheckOutRepository _checkoutRepository = null;
    public ICheckOutRepository CheckoutRepository
    {
      get
      {
        if (_checkoutRepository == null)
        {
          IUnityContainer container = new UnityContainer();
          container.RegisterType<IAstraContext, AstraContext>();
          container.RegisterType<ICheckOutRepository, CheckOutRepository>();
          _checkoutRepository = container.Resolve<ICheckOutRepository>();

          _disposeList.Add(_checkoutRepository);
        }
        return _checkoutRepository;
      }
    }

    private IUserProfileRepository _userProfileRepository = null;
    public IUserProfileRepository UserProfileRepository
    {
      get
      {
        if (_userProfileRepository == null)
        {
          IUnityContainer container = new UnityContainer();
          container.RegisterType<IAstraContext, AstraContext>();
          container.RegisterType<IUserProfileRepository, UserProfileRepository>();
          _userProfileRepository = container.Resolve<IUserProfileRepository>();

          _disposeList.Add(_userProfileRepository);
        }
        return _userProfileRepository;
      }
    }

    private IMiscRepository _miscRepository = null;
    public IMiscRepository MiscRepository
    {
      get
      {
        if (_miscRepository == null)
        {
          IUnityContainer container = new UnityContainer();
          container.RegisterType<IAstraContext, AstraContext>();
          container.RegisterType<IMiscRepository, MiscRepository>();
          _miscRepository = container.Resolve<IMiscRepository>();

          _disposeList.Add(_miscRepository);
        }
        return _miscRepository;
      }
    }

    private IResourceTypeRepository _resourceTypeRepository = null;
    public IResourceTypeRepository ResourceTypeRepository
    {
      get
      {
        if (_resourceTypeRepository == null)
        {
          IUnityContainer container = new UnityContainer();
          container.RegisterType<IAstraContext, AstraContext>();
          container.RegisterType<IResourceTypeRepository, ResourceTypeRepository>();
          _resourceTypeRepository = container.Resolve<IResourceTypeRepository>();

          _disposeList.Add(_resourceTypeRepository);
        }
        return _resourceTypeRepository;
      }
    }

    private IKeyWordLinksRepository _keywordLinksRepository = null;
    public IKeyWordLinksRepository KeyWordLinksRepository
    {
      get
      {
        if (_keywordLinksRepository == null)
        {
          IUnityContainer container = new UnityContainer();
          container.RegisterType<IAstraContext, AstraContext>();
          container.RegisterType<IKeyWordLinksRepository, KeyWordLinksRepository>();
          _keywordLinksRepository = container.Resolve<IKeyWordLinksRepository>();

          _disposeList.Add(_keywordLinksRepository);
        }
        return _keywordLinksRepository;
      }
    }

    private ICommentRepository _commentRepository = null;
    public ICommentRepository CommentRepository
    {
      get
      {
        if (_commentRepository == null)
        {
          IUnityContainer container = new UnityContainer();
          container.RegisterType<IAstraContext, AstraContext>();
          container.RegisterType<ICommentRepository, CommentRepository>();
          _commentRepository = container.Resolve<ICommentRepository>();

          _disposeList.Add(_commentRepository);
        }
        return _commentRepository;
      }
    }

    private IRatingsRepository _ratingRepository = null;
    public IRatingsRepository RatingsRepository
    {
      get
      {
        if (_ratingRepository == null)
        {
          IUnityContainer container = new UnityContainer();
          container.RegisterType<IAstraContext, AstraContext>();
          container.RegisterType<IRatingsRepository, RatingsRepository>();
          _ratingRepository = container.Resolve<IRatingsRepository>();

          _disposeList.Add(_ratingRepository);
        }
        return _ratingRepository;
      }
    }

    private IKeywordRepository _keywordRepository;
    public IKeywordRepository KeywordRepository
    {
      get
      {
        if (_keywordRepository == null)
        {
          IUnityContainer container = new UnityContainer();
          container.RegisterType<IAstraContext, AstraContext>();
          container.RegisterType<IKeywordRepository, KeywordRepository>();
          _keywordRepository = container.Resolve<IKeywordRepository>();

          _disposeList.Add(_keywordRepository);
        }
        return _keywordRepository;
      }
    }

    private ISuggestionRepository _suggestionRepository;
    public ISuggestionRepository SuggestionRepository
    {
      get
      {
        if (_suggestionRepository == null)
        {
          IUnityContainer container = new UnityContainer();
          container.RegisterType<IAstraContext, AstraContext>();
          container.RegisterType<ISuggestionRepository, SuggestionRepository>();
          _suggestionRepository = container.Resolve<ISuggestionRepository>();

          _disposeList.Add(_suggestionRepository);
        }
        return _suggestionRepository;
      }
    }

    private IReservationRepository _reservationRepository;
    public IReservationRepository ReservationRepository
    {
      get
      {
        if (_reservationRepository == null)
        {
          IUnityContainer container = new UnityContainer();
          container.RegisterType<IAstraContext, AstraContext>();
          container.RegisterType<IReservationRepository, ReservationRepository>();
          _reservationRepository = container.Resolve<IReservationRepository>();

          _disposeList.Add(_reservationRepository);
        }
        return _reservationRepository;
      }
    }

    private IUserMailMessageRepository _userMessageRepository;
    public IUserMailMessageRepository UserMessageRepository
    {
      get
      {
        if (_userMessageRepository == null)
        {
          IUnityContainer container = new UnityContainer();
          container.RegisterType<IAstraContext, AstraContext>();
          container.RegisterType<IUserMailMessageRepository, UserMailMessageRepository>();
          _userMessageRepository = container.Resolve<IUserMailMessageRepository>();

          _disposeList.Add(_userMessageRepository);
        }
        return _userMessageRepository;
      }
    }

    private IReportRepository _reportRepository;
    public IReportRepository ReportRepository
    {
      get
      {
        if (_reportRepository == null)
        {
          IUnityContainer container = new UnityContainer();
          container.RegisterType<IAstraContext, AstraContext>();
          container.RegisterType<IReportRepository, ReportRepository>();
          _reportRepository = container.Resolve<IReportRepository>();

          _disposeList.Add(_reportRepository);
        }
        return _reportRepository;
      }
    }

    private IAuditRepository _auditRepository  = null;
    public IAuditRepository AuditRepository
    {
      get 
      {
        if (_auditRepository==null)
        {
          IUnityContainer container = new UnityContainer();
          container.RegisterType<IAstraContext, AstraContext>();
          container.RegisterType<IAuditRepository, AuditRepository>();
          _auditRepository = container.Resolve<IAuditRepository>();

          _disposeList.Add(_auditRepository);
        }
        return _auditRepository;
      }
      
    }


    #endregion 

    #region IDisposable

    protected virtual void Dispose(bool disposing)
    {
      if (!this._disposed)
      {
        if (disposing)
        {
          foreach (var item in _disposeList)
          {
            item.Dispose();
          }

          _context.Dispose();
        }
      }
      this._disposed = true;
    }

    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    #endregion 
  }
}