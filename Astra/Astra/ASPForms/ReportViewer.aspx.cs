using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Astra.AstraConfigurations.Settings;
using Astra.CompositeRepository;
using Astra.Models;
using MTUtil.TypeManagement;
using MSR = Microsoft.Reporting.WebForms;

namespace Astra.ASPForms
{
  public partial class ReportViewer : System.Web.UI.Page
  {
    protected void Page_Load(object sender, EventArgs e)
    {
      if (!IsPostBack)
      {
        int reportId = TypeUtils.ToInt(Request.QueryString["ReportId"]);

        Report report = null;
        using (var composite = new ScopedCompositeRepository())
        {
          report = composite.Repositories.ReportRepository.Find(reportId);
        }
        
        ReportSettings settings = (ReportSettings)System.Configuration.ConfigurationManager.GetSection("reportSettings");

        ReportViewer1.ProcessingMode = MSR.ProcessingMode.Remote;
        ReportViewer1.ServerReport.ReportServerUrl = new Uri(settings.ReportServerUrl);
        ReportViewer1.ServerReport.ReportPath = report.ReportPath;
      }
    }
  }
}