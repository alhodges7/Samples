using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;

namespace Astra.NotificationServices
{
  public static class CancellationTokenLibrary
  {
    private static CancellationTokenSource _notificationsCancellationTokenSource;
    private static CancellationToken _notificationsCancelationToken;

    public static CancellationTokenSource NotificationsCancellationTokenSource
    {
      get
      {
        if (_notificationsCancellationTokenSource == null)
        {
          _notificationsCancellationTokenSource = new CancellationTokenSource();
          _notificationsCancelationToken = _notificationsCancellationTokenSource.Token;
        }
        return _notificationsCancellationTokenSource;
      }
    }
    public static CancellationToken NotificationsCancelationToken
    {
      get
      {
        if (_notificationsCancellationTokenSource == null)
        {
          _notificationsCancellationTokenSource = new CancellationTokenSource();
          _notificationsCancelationToken = _notificationsCancellationTokenSource.Token;
        }
        return _notificationsCancelationToken;
      }
    }

  }
}