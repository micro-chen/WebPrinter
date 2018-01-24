using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;
using System.Linq;
using System.Threading;

namespace SmartClient.Bootstraper
{
    /// <summary>
    /// 隐藏的程序启动上下文
    /// </summary>
    internal class HideOnStartupApplicationContext : ApplicationContext
    {
        /// <summary>
        /// 当前程序集名称
        /// </summary>
        public static string AppName = Assembly.GetExecutingAssembly().GetName().Name;

        private Form mainFormInternal;// 构造函数，主窗体被存储在mainFormInternal
        public HideOnStartupApplicationContext(Form mainForm)
        {

            this.mainFormInternal = mainForm;// 当主窗体被关闭时，退出应用程序

            mainForm.ShowInTaskbar = false;
            mainForm.WindowState = FormWindowState.Minimized;
            mainForm.FormClosed += mainFormInternal_Closed;


            this.mainFormInternal.Show();//为了触发窗体的Load事件
        }
        void mainFormInternal_Closed(object sender, EventArgs e)
        {
            //Application.Exit();
            System.Environment.Exit(0);//终止程序进程 及其子进程
        }

        /// <summary>
        /// 当前程序是否已经运行中
        /// 检测此程序的运行数目是否大于1 超过1个
        /// 并对比当前进程跟检测的目标进程的id
        /// 就是正在运行中
        /// </summary>
        /// <returns></returns>
        public static bool IsCurrentAppProcessHasRunning()
        {


            var result = false;

            int maxCount = 1;
            int currentPsId = Process.GetCurrentProcess().Id;
            
            var psList = Process.GetProcessesByName(AppName);

            int psCount = 0;
            if (null != psList && psList.Length > 0)
            {
                for (int i = 0; i < psList.Length; i++)
                {
                    var tmpPs = psList[i];
                    if (tmpPs.Id==currentPsId)
                    {
                        continue;
                    }

                    psCount++;//非本进程的其他同名 非同id进程 
                }
            }



            if (psCount > maxCount)
            {
                result = true;
            }

            return result;
        }

    }
}
