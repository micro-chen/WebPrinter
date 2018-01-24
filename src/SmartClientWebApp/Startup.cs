using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Owin;
using Owin;
using System.Web.Http;
using Microsoft.Owin.Cors;
using System.Web.Cors;
using Microsoft.AspNet.SignalR;

[assembly: OwinStartup(typeof(SmartClient.Web.Startup))]

namespace SmartClient.Web
{
    public partial class Startup
    {
        //正式环境中 ，可以进CORS的域名
        public static List<string> AuthedDomains
        {
            get
            {
                return new List<string>()
                {
                    "http://localhost:8066",
                    "http://127.0.0.1:8066",
                    "http://localhost:6699",
                    "http://127.0.0.1:6699",
                    "http://smartclient.com",
                    "http://www.smartclient.com"
                };
            }
        }

        ///// <summary>
        ///// 返回一个设定好的可以跨域的域名提供
        ///// </summary>
        ///// <returns></returns>
        //private CorsOptions InitAuthedPolicyProvider()
        //{
        //    var corsOption = new CorsOptions();
        //    var policy = new CorsPolicy
        //    {
        //        AllowAnyHeader = true,
        //        AllowAnyMethod = true,
        //        AllowAnyOrigin = false,
        //        SupportsCredentials = true
        //    };
        //    policy.Origins.AddRange(AuthedDomains);
        //    ICorsPolicyProvider corsPolicyProvider = new CorsPolicyProvider()
        //    {
        //        PolicyResolver = context => TaskHelpers.FromResult(policy)

        //    };

        //    corsOption.PolicyProvider = corsPolicyProvider;
        //    return corsOption;
        //}

        public void Configuration(IAppBuilder app)
        {
            /*
             *   By default, SignalR retains 1000 messages in memory per hub per connection. If large messages are being used, this may create memory issues which can be alleviated by reducing this value. This setting can be set in the Application_Start event handler in an ASP.NET application, or in the Configuration method of an OWIN startup class in a self-hosted application.
             *   最小值为32 MessageStore capacity
             */
            //设定SignalR的单个连接的消息数目的最大数，降低内存占用体积
            GlobalHost.Configuration.DefaultMessageBufferSize = 32;

            // ConfigureAuth(app);
            // Configure Web API for self-host.  
            HttpConfiguration config = new HttpConfiguration();
            
            WebApiConfig.Register(config);


            app.UseCors(CorsOptions.AllowAll);


            

            app.UseWebApi(config);

            //app.MapSignalR();//this is signalR 2.X
            app.MapHubs();//Signal 1.x
        }
    }


   
}
