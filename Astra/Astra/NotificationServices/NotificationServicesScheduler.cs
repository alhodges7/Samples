using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Routing;
using Astra.CompositeRepository;
using Astra.AstraConfigurations;

namespace Astra.NotificationServices
{
  public class NotificationServicesScheduler
  {
    private Task _overdueCheckService;
    private Task _newBookNotifyService;
   
    public NotificationServicesScheduler()
    {
      
      if (!CancellationTokenLibrary.NotificationsCancelationToken.IsCancellationRequested)
      {
        // Start Services
        InitializeServices();
        StartServices();

      }

    }
    private void InitializeServices()
    {
      _overdueCheckService = new Task(() => new OverdueCheckService());
      _newBookNotifyService = new Task(() => new NewResourceNotificationService());
    }
    private void StartServices()
    {
      if (AstraConfigurationManager.OverdueBookSettings().EnableService.ToLower()=="yes")
      {
        _overdueCheckService.Start();
      }
      if (AstraConfigurationManager.NewResourceSettings().EnableService.ToLower() == "yes")
      {
        _newBookNotifyService.Start();
      }

    }





  }
}