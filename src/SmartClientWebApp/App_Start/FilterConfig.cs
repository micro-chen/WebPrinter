using System.Web;
using System.Web.Mvc;
using SmartClient.Web.Common;

namespace SmartClient.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}