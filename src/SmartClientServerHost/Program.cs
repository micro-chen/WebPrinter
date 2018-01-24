using System;


using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Owin.Hosting;
using Newtonsoft.Json;
using System.Timers;

using SmartClient.Web;
using SmartClient.Model;



namespace SmartClient.ServerHost
{
    class Program
    {

        private static IDisposable appServer = null;
        private static Timer timerFoKeepAlive = null;

        /// <summary>
        /// 频率进行定时的请求IIS /Mono /Owin  防止休眠
        /// </summary>
        private const int TickFrequencyFoKeepAlive = 15*1000;

        //////        //install-package microsoft.owin.hosting
        //////        //install-package Microsoft.Owin.Host.HttpListener

        //////   StartOptions options = new StartOptions();
        //////   options.Urls.Add("http://localhost:5000/");
        ////////options.Urls.Add(string.Format("http://{0}:5000", Environment.MachineName));
        ////////options.Urls.Add("http://+:5000/");
        ////////options.Urls.Add("http://*:5000/");

        //////using (WebApp.Start<WebAPISelfHostMinimal.Startup>(options))
        //////{
        //////    while (!Terminate)
        //////    {
        //////        await Task.Delay(10); //keep cpu from getting pegged
        //////    }

        //////    LogUtil.LogInfo("Terminating owin host.");
        //////}


        static void Main(string[] args)
        {
            try
            {



                // Start OWIN host  
                //using (WebApp.Start<Startup>(url: baseAddress))
                //{ // Create HttpCient and make a request to api/values  
                //    HttpClient client = new HttpClient();
                //    var response = client.GetAsync(baseAddress + "api/values").Result;

                //    Console.WriteLine(response);
                //    Console.WriteLine(response.Content.ReadAsStringAsync().Result);
                //}

                appServer = WebApp.Start<Startup>(GlobalConfig.BaseBindingAddress);

                Console.WriteLine("we are ready......press enter to end");

                Task.Factory.StartNew(() =>
                {
                    LoadingOneRequestTest();


                    //启动定时器 开始每间隔一定的事件 发送一次请求 防止进程被回收
                    timerFoKeepAlive = new Timer(TickFrequencyFoKeepAlive);
                    timerFoKeepAlive.Elapsed += (object sender, ElapsedEventArgs e) =>
                    {
                        LoadingOneRequestTest();
                    };

                    timerFoKeepAlive.Start();
                });

            }
            catch
            {
                throw;
            }
            finally { }
            Console.ReadLine();

            //注销定时器
            timerFoKeepAlive.Stop();
            timerFoKeepAlive.Dispose();

            if (null != appServer)
            {
                appServer.Dispose();
            }
        }



        /// <summary>
        /// 启动后 尝试向程序发送一次HTTP请求，用来进行程序的sgen
        /// </summary>
        static void LoadingOneRequestTest()
        {
            string keepAliveAddr = string.Concat(GlobalConfig.BaseAddress, "/api/System/KeepAlive");

            try
            {


                var client = new HttpClient();
                //设定refer 进行自身的安全验证，域控限制
                client.DefaultRequestHeaders.Referrer = new Uri(GlobalConfig.BaseAddress);
                client.DefaultRequestHeaders.From = SmartClient.Web.Common.ScurityFilter.IsFromClientToken;
                string result = string.Empty;
                var tsk = client.GetStringAsync(keepAliveAddr);
                //tsk.Wait();
                //执行完毕后，显示  
                result = tsk.Result;
                if (!string.IsNullOrEmpty(result))
                {
                    var msg = JsonConvert.DeserializeObject<MessageConteiner<string>>(result);
                    Console.WriteLine("the first init request result is:{0}", msg.Message);
                    return;
                }

                Console.WriteLine("Sorry,the first init request error!!!!");




            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());

            }


        }


    }
}
