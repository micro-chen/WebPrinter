using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace SmartClient.Bootstraper
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            //0 在host 项目中 检测此项目是否已经启动 如果已经启动 那么发送请求到此 否则启动这个进程。hook 是一个自启动Windows服务。提供基础的host的监视保护钩子
            //1程序启动的时候 检测是否同名的进程已经启动，不允许多个进程存在
            //2 获取完毕UAC后 启动另一个端口的 http 监听，对控制器中的Action进行监听
            //3 类似owin host的引用 
            //检测授权完毕。
          

            var startForm = new MainForm();
            HideOnStartupApplicationContext context = new HideOnStartupApplicationContext(startForm);
            Application.Run(context);//隐藏窗体的方法
        }
    }

}
