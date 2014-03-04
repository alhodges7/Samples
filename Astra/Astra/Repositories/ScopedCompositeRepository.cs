using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Astra.Logging;

namespace Astra.CompositeRepository
{
  public class ScopedCompositeRepository : IDisposable
  {
    [ThreadStatic]
    private CompositeRepository _instance = null;

    public ScopedCompositeRepository()
    {
      if (null == _instance)
      {
        _instance = new CompositeRepository();
      }
    }

    public CompositeRepository Repositories
    {
      get
      {
        return _instance;
      }
    }

    private bool _disposed = false;

    protected virtual void Dispose(bool disposing)
    {
      AstraLogger.LogDebug("Disposing...");

      if (!this._disposed)
      {
        if (disposing)
        {
          Repositories.Dispose();
        }
      }
      this._disposed = true;

      AstraLogger.LogDebug("Dispose Complete.");
    }

    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }
  }
}