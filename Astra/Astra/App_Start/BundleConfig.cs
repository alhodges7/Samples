using System.Web;
using System.Web.Optimization;

namespace Astra
{
  public class BundleConfig
  {
    // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
    public static void RegisterBundles(BundleCollection bundles)
    {
      bundles.Add(new ScriptBundle("~/bundles/spinner").Include(
                  "~/Scripts/spin.js",
                  "~/Scripts/Spinner.js"
                  ));

      bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                  "~/Scripts/jquery-{version}.js"
                  ));

      bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                  "~/Scripts/jquery-ui-1.10.2.js",
                  "~/Scripts/jquery-ui-1.10.2.min.js"
                  ));

      bundles.Add(new ScriptBundle("~/bundles/fancybox").Include(
                  "~/Scripts/fancybox/jquery.fancybox.pack.js",
                  "~/Scripts/fancybox/helpers/jquery.fancybox-buttons.js",
                  "~/Scripts/fancybox/helpers/jquery.fancybox-media.js",
                  "~/Scripts/fancybox/helpers/jquery.fancybox-thumbs.js"));

      bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                  "~/Scripts/jquery.unobtrusive*",
                  "~/Scripts/jquery.validate*"));

      bundles.Add(new ScriptBundle("~/bundles/rateitjs").Include(
                 "~/Scripts/rateit/jquery.rateit.js",
                  "~/Scripts/rateit/rateresource.js"
                 ));

      // Use the development version of Modernizr to develop with and learn from. Then, when you're
      // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
      bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                  "~/Scripts/modernizr-*"));

      bundles.Add(new StyleBundle("~/Content/css").Include(
                  "~/Content/site.css",
                  "~/Content/PagedList.css"));


      bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
                  "~/Content/themes/base/jquery.ui.core.css",
                  "~/Content/themes/base/jquery.ui.resizable.css",
                  "~/Content/themes/base/jquery.ui.selectable.css",
                  "~/Content/themes/base/jquery.ui.accordion.css",
                  "~/Content/themes/base/jquery.ui.autocomplete.css",
                  "~/Content/themes/base/jquery.ui.button.css",
                  "~/Content/themes/base/jquery.ui.dialog.css",
                  "~/Content/themes/base/jquery.ui.slider.css",
                  "~/Content/themes/base/jquery.ui.tabs.css",
                  "~/Content/themes/base/jquery.ui.datepicker.css",
                  "~/Content/themes/base/jquery.ui.progressbar.css",
                  "~/Content/themes/base/jquery.ui.theme.css",
                  "~/Content/Spinner.css" 
                  ));

      bundles.Add(new StyleBundle("~/Content/AstraStyles").Include(
                  "~/Content/AstraStyles.css"));

      bundles.Add(new StyleBundle("~/Content/fancybox").Include(
                  "~/Content/fancybox/jquery.fancybox.css",
                  "~/Content/fancybox/helpers/jquery.fancybox-buttons.css",
                  "~/Content/fancybox/helpers/jquery.fancybox-thumbs.css"));

      bundles.Add(new StyleBundle("~/Content/rateitcss").Include(
                 "~/Content/rateit/rateit.css"));

      BundleTable.EnableOptimizations = true;

    }
  }
}