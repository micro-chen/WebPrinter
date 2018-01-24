
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Hosting;
using System.Web.Http;
using SmartClient.Web.Common;


namespace SmartClient.Web
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            //var formatters = config.Formatters;
            //config.Formatters.Remove(formatters.XmlFormatter);
            var jsonFormatter = config.Formatters.JsonFormatter;
            config.Formatters.Clear();
            config.Formatters.Add(jsonFormatter);



            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );



            ///应用是否由IIS OR IIS Express 承载.如果是自承载 那么进行域的安全验证，否则，在iis express等，属于调试范围！不做验证
            //暂时不添加域验证
            //if (!HostingEnvironment.IsHosted)
            //{
            //    //CORS二次验证过滤器
            //    config.Filters.Add(new ScurityFilter());
            //}


            // 取消注释下面的代码行可对具有 IQueryable 或 IQueryable<T> 返回类型的操作启用查询支持。
            // 若要避免处理意外查询或恶意查询，请使用 QueryableAttribute 上的验证设置来验证传入查询。
            // 有关详细信息，请访问 http://go.microsoft.com/fwlink/?LinkId=279712。
            //config.EnableQuerySupport();

            // 若要在应用程序中禁用跟踪，请注释掉或删除以下代码行
            // 有关详细信息，请参阅: http://www.asp.net/web-api
            //config.EnableSystemDiagnosticsTracing();
        }
    }
}
