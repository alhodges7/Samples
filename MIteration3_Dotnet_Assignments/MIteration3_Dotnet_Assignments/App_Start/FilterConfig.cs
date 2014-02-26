using System.Web;
using System.Web.Mvc;

namespace MIteration3_Dotnet_Assignments
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}