
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Win32;
using System.IO;
using SmartClient.Model;
using SmartClient.Common;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

/*通过索引遍历的方式 迭代进程数组 集合对象 要比Linq快很多*/
namespace SmartClient.Common.Extensions
{
    public class SystemAppExtension
    {
        /// <summary>
        /// 菜鸟组件的执行文件名称
        /// </summary>
        static readonly string CaiNiaoExcutePath = "CNPrintClient.exe";
        static readonly string CaiNiaoPrintName = "CaiNiao打印组件";
        static readonly string CaiNiaoPrintProcessName = "CNPrintClient";
        static readonly string CaiNiaoPrintMonitorProcessName = "CNPrintMonitor";
        //工作台的基本信息
        static readonly string WorkBenchProcessName = "SmartClient.Bootstraper";
        static readonly string WorkBenchExcutePath = "SmartClient.Bootstraper.exe";
        static readonly bool IsMono = Type.GetType("Mono.Runtime") != null;

        /// <summary>
        /// 内置菜鸟的路径
        /// </summary>
        public static string DefaultInnerCaiNiaoInstallPath
        {
            get
            {
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory.Replace("app", "CaiNiao"), CaiNiaoExcutePath);
            }
        }

        /// <summary>
        /// 默认内置菜鸟的版本
        /// </summary>
        public static string DefaultInnerCaiNiaoVersion
        {
            get
            {
                var fvi =FileVersionInfo.GetVersionInfo(DefaultInnerCaiNiaoInstallPath);
                if (null == fvi)
                {
                    return string.Empty;
                }
                return fvi.FileVersion;
            }
        }
        /// <summary>
        /// 菜鸟正在运行状态
        /// </summary>
        const int IS_RUNING = 1;
        #region WINAPI

        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool SetDefaultPrinter(string printerName);

        /// <summary>
        /// //调用Windows API 展示窗口到最前端
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="fAltTab"></param>
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern void SwitchToThisWindow(IntPtr hWnd, bool fAltTab);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass,
            string lpszWindow);

        [DllImport("user32.dll")]
        public static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint msg, int wParam, int lParam);


        #endregion

        /// <summary>
        /// 通过打印机名称设定默认打印机-------无法通过系统用户操作 当前登录用户
        /// </summary>
        /// <param name="printerName"></param>
        /// <returns></returns>
        public static int SetDefaultPrinterByName(string printerName)
        {
            int result = 0;
            try
            {
                if (string.IsNullOrEmpty(printerName))
                {
                    throw new Exception("必须输出指定的打印机名称！不能为空！");
                }

                //var tagetRegistryItem = "Software\\Microsoft\\Windows NT\\CurrentVersion\\Windows";
                //var targetKeyItem = "Device";

                //if (!RegistryHelper.IsRegistryExist(Registry.CurrentUser, tagetRegistryItem, targetKeyItem))
                //{
                //    throw new Exception("未能获取系统设备项列表！");
                //}



                ////当前设定的打印设备值
                //var currentDeviceValue = RegistryHelper.GetRegistryData(Registry.CurrentUser, tagetRegistryItem, targetKeyItem);
                //StringBuilder sbToWriteToDevice = new StringBuilder(printerName);

                //if (!string.IsNullOrEmpty(currentDeviceValue))
                //{
                //    //已经设置过打印机列表，那么更换
                //    var dvs = currentDeviceValue.Split(',');
                //    for (int i = 0; i < dvs.Length; i++)
                //    {
                //        if (i == 0)
                //        {
                //            continue;//忽略默认的第一个。第一个是默认打印机
                //        }
                //        string item = dvs[i];
                //        sbToWriteToDevice.AppendFormat(",{0}", item);
                //    }
                //}

                //string defaultPrintNameOfCurrent = sbToWriteToDevice.ToString();

                //var operateResult = RegistryHelper.SetRegistryData(Registry.CurrentUser, tagetRegistryItem, targetKeyItem, defaultPrintNameOfCurrent);//SystemAppExtension.SetDefaultPrinter(printerName);
                //if (false == operateResult)
                //{
                //    var errCode = Marshal.GetLastWin32Error();
                //    var errMsg = string.Format("设定默认打印机失败！错误码:{0}。打印机名称:{1}。", errCode, printerName);
                //    throw new Exception(errMsg);
                //}

                var operateResult = SetDefaultPrinter(printerName);
                if (false == operateResult)
                {
                    var errCode = Marshal.GetLastWin32Error();
                    var errMsg = string.Format("设定默认打印机失败！错误码:{0}。打印机名称:{1}。", errCode, printerName);
                    throw new Exception(errMsg);
                }

                result = 1;
            }
            catch (Exception ex)
            {
                throw new BusinessException("设定默认打印机失败出现异常，异常信息：{0}", ex.Message);

            }

            return result;
        }

        #region 工作台相关



        /// <summary>
        /// 工作台是否运行中
        /// </summary>
        /// <returns></returns>
        public static int IsWorkBenchIsRunning()
        {

            var result = 0;

            try
            {
                var ps = Process.GetProcessesByName(WorkBenchProcessName);
                if (null != ps && ps.Length >= 1)
                {
                    result = 1;
                }

            }
            catch (Exception ex)
            {

                throw new BusinessException(ex.Message);
            }


            return result;

        }
        /// <summary>
        /// 运行工作台
        /// </summary>
        /// <returns></returns>
        public static int StartWorkBench()
        {
            int result = 0;

            //如果菜鸟组件正在运行中 那么直接返回
            if (IsWorkBenchIsRunning() == IS_RUNING)
            {
                return 1;
            }

            try
            {
                var workBenchInstallPath = string.Empty;
                //当前程序集下的工作台执行文件
                workBenchInstallPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, WorkBenchExcutePath);

                if (!File.Exists(workBenchInstallPath))
                {
                    throw new Exception("未能检测到智能客户端工作台的安装！请从新安装拓展SDK！");

                }


                var ps = ProcessUtil.CreateProcessAsUser(workBenchInstallPath, "", ProcessUtil.SESSION_TYPE.SessionFromProcessExplorerSession);
                if (null == ps)
                {
                    throw new BusinessException("未能正确启动，智能客户端工作台启动失败!");
                }

                result = 1;

            }
            catch (Exception ex)
            {
                throw new BusinessException("开启智能客户端工作台运行出现异常，异常信息：{0}", ex.Message);

            }

            return result;
        }
        #endregion

        /// <summary>
        /// 打开菜鸟运行
        /// </summary>
        /// <returns></returns>
        public static int SatrtCaiNiaoPrinter()
        {
            int result = 0;

            //如果菜鸟组件正在运行中 那么直接返回
            if (CheckCaiNiaoPrinterStatus() == IS_RUNING)
            {
                return 1;
            }

            try
            {
                var caiNiaoInstallPath = string.Empty;
                SoftwareInfo model;
                var resultCheck = SystemAppExtension.IsCaiNiaoPrintInstalled(out model);
                if (resultCheck == false)
                {
                    // throw new Exception("未能检测到菜鸟的安装！请从新安装菜鸟打印组件！");
                    //未能检测到菜鸟的安装  那么使用内置的菜鸟组件
                    model = new SoftwareInfo
                    {
                        InstallLocation = Path.Combine(AppDomain.CurrentDomain.BaseDirectory.Replace("app", "CaiNiao"), CaiNiaoExcutePath)
                    };//替换到内置菜鸟组件目录
                }

                caiNiaoInstallPath = model.InstallLocation;

                var psInfo = new ProcessStartInfo { FileName = caiNiaoInstallPath, UseShellExecute = true };
                Process.Start(psInfo);


                //var ps = ProcessUtil.CreateProcessAsUser(model.InstallLocation, "", ProcessUtil.SESSION_TYPE.SessionFromProcessExplorerSession);
                //if (null == ps)
                //{
                //    throw new BusinessException("未能正确启动，系统服务启动失败！请尝试手工运行菜鸟组件！");
                //}

                // ProcessHelper.CreateProcess(@"C:\Program Files (x86)\CaiNiao打印组件\CNPrintClient.exe", @"C:\Program Files (x86)\CaiNiao打印组件\");
                

            }
            catch (Exception ex)
            {
                throw new BusinessException("开启菜鸟打印组件运行出现异常，异常信息：{0}", ex.Message);

            }finally
            {
                if (result!=1)
                {
                    var localModel = new SoftwareInfo
                    {
                        InstallLocation = Path.Combine(AppDomain.CurrentDomain.BaseDirectory.Replace("app", "CaiNiao"), CaiNiaoExcutePath)
                    };//替换到内置菜鸟组件目录
                    var psInfo = new ProcessStartInfo { FileName = localModel.InstallLocation, UseShellExecute = true };
                    Process.Start(psInfo);

                }


                result = CheckCaiNiaoPrinterStatus();
            }

            return result;
        }



        /// <summary>
        /// 重启菜鸟。
        /// 菜鸟一旦终止就不会再启动。所以 终止程序，让监视程序启动
        /// </summary>
        /// <returns></returns>
        public static int RestartCaiNiaoPrinter()
        {
            int result = 0;
            try
            {

                if (CheckCaiNiaoPrinterStatus() != IS_RUNING)
                {
                    throw new Exception("未能检索到菜鸟组件的运行！");
                }



                var caiNiaoProcess = Process.GetProcessesByName(CaiNiaoPrintProcessName).FirstOrDefault();
                if (null != caiNiaoProcess)
                {
                    caiNiaoProcess.Kill();
                    result = 1;
                }

            }
            catch (Exception ex)
            {
                throw new BusinessException("关闭重启菜鸟打印组件运行出现异常，异常信息：{0}", ex.Message);

            }

            return result;
        }

        /// <summary>
        /// 终止菜鸟运行
        /// </summary>
        /// <returns></returns>
        public static int TerminalCaiNiaoPrinter()
        {
            int result = 0;
            try
            {

                if (CheckCaiNiaoPrinterStatus() != IS_RUNING)
                {
                    throw new Exception("未能检索到菜鸟组件的运行！");
                }

                var allProcessList = Process.GetProcesses();
                int counter = 0;
                try
                {
                    foreach (var item in allProcessList)
                    {
                        if (item.ProcessName == CaiNiaoPrintProcessName || item.ProcessName == CaiNiaoPrintMonitorProcessName)
                        {
                            item.Kill();
                            counter += 1;
                        }

                        if (counter == 2)
                        {
                            break;//菜鸟一共两个进程
                        }

                    }

                    //杀死进程后 刷新托盘区域 
                    RefreshTrayArea();
                    result = 1;
                }
                catch (Exception ex)
                {
                    throw ex;
                }


            }
            catch (Exception ex)
            {
                throw new BusinessException("关闭重启菜鸟打印组件运行出现异常，异常信息：{0}", ex.Message);

            }

            return result;
        }


        /// <summary>
        /// 检测菜鸟打印组件是否正在运行
        /// </summary>
        /// <param name="modelInfo"></param>
        /// <returns></returns>
        public static int CheckCaiNiaoPrinterStatus()
        {
            int result = 0;
            try
            {
                //SoftwareInfo modelSoft;
                //if (!IsCaiNiaoPrintInstalled(out modelSoft))
                //{
                //    //未能检索到菜鸟的安装
                //    throw new Exception("未能检索到菜鸟组件的安装。请先下载安装。下载地址：http://cloudprint-docs-resource.oss-cn-shanghai.aliyuncs.com/download.html");
                //}

                var ps = Process.GetProcessesByName(CaiNiaoPrintProcessName);
                if (null != ps && ps.Length >= 1)
                {
                    result = 1;
                }

            }
            catch (Exception ex)
            {
                throw new BusinessException("检测菜鸟打印组件运行状态出现异常，异常信息：{0}", ex.Message);

            }

            return result;
        }

        /// <summary>
        /// 检测菜鸟打印组件是否安装到客户机
        /// </summary>
        /// <param name="modelInfo"></param>
        /// <returns></returns>
        public static bool IsCaiNiaoPrintInstalled(out SoftwareInfo modelInfo)
        {
            bool result = false;


            modelInfo = null;



            RegistryKey[] keys = new RegistryKey[] {
             // search in: CurrentUser
             Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall"),
            // search in: LocalMachine_32
              Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall"),
            // search in: LocalMachine_64
              Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall")
            };

            bool isCanStop = false;

            foreach (var keyItem in keys)
            {
                if (isCanStop == true)
                {
                    break;
                }
                var key = keyItem;
                if (null != key)
                {
                    foreach (String keyName in key.GetSubKeyNames())
                    {
                        RegistryKey subkey = key.OpenSubKey(keyName);
                        string displayName = subkey.GetValue("DisplayName") as string;

                        if (!string.IsNullOrEmpty(displayName) && displayName.Contains(CaiNiaoPrintName))
                        {
                            modelInfo = SystemAppExtension.InitSoftInfoModelFromRegistryKey(ref key, keyName);

                            //获取安装路径
                            if (string.IsNullOrEmpty(modelInfo.InstallLocation))
                            {
                                if (!string.IsNullOrEmpty(modelInfo.UninstallString))
                                {
                                    string installDir = Path.GetDirectoryName(modelInfo.UninstallString);
                                    modelInfo.InstallLocation = Path.Combine(installDir, CaiNiaoExcutePath);
                                }
                            }
                            result = true;
                            isCanStop = true;
                            break;
                        }


                    }
                }
            }

            return result;

        }

        /// <summary>
        /// 从一个注册表项中  初始化一个安装的软件的信息
        /// </summary>
        /// <param name="key"></param>
        /// <param name="keyName"></param>
        /// <returns></returns>
        internal static SoftwareInfo InitSoftInfoModelFromRegistryKey(ref RegistryKey key, string keyName)
        {
            var model = new SoftwareInfo();
            if (null == key)
            {
                return null;
            }

            try
            {


                RegistryKey subkey = key.OpenSubKey(keyName);
                if (null != subkey.GetValue("DisplayName"))
                {
                    model.DisplayName = subkey.GetValue("DisplayName").ToString();
                }

                if (null != subkey.GetValue("DisplayVersion"))
                {
                    model.DisplayVersion = subkey.GetValue("DisplayVersion").ToString();
                }
                if (null != subkey.GetValue("InstallDate"))
                {
                    model.InstallDate = subkey.GetValue("InstallDate").ToString();
                }
                if (null != subkey.GetValue("Publisher"))
                {
                    model.Publisher = subkey.GetValue("Publisher").ToString();
                }
                if (null != subkey.GetValue("InstallLocation"))
                {
                    model.InstallLocation = subkey.GetValue("InstallLocation").ToString();
                }
                if (null != subkey.GetValue("Version"))
                {
                    model.Version = subkey.GetValue("Version").ToString();
                }
                if (null != subkey.GetValue("VersionMinor"))
                {
                    model.VersionMinor = subkey.GetValue("VersionMinor").ToString();
                }
                if (null != subkey.GetValue("VersionMajor"))
                {
                    model.VersionMajor = subkey.GetValue("VersionMajor").ToString();
                }
                if (null != subkey.GetValue("EstimatedSize"))
                {
                    model.EstimatedSize = subkey.GetValue("EstimatedSize").ToString();
                }
                if (null != subkey.GetValue("UninstallString"))
                {
                    model.UninstallString = subkey.GetValue("UninstallString").ToString();
                }
                if (null != subkey.GetValue("HelpLink"))
                {
                    model.HelpLink = subkey.GetValue("HelpLink").ToString();
                }


            }
            catch (Exception ex)//if (appName.Equals(displayName, StringComparison.OrdinalIgnoreCase) == true)
            //{
            //    return true;
            //}
            {

                throw ex;
            }
            return model;
            //System.Diagnostics.Debug.WriteLine(displayName);


        }

        /// <summary>
        /// 获取系统中已经安装的软件列表
        /// </summary>
        /// <returns></returns>
        public static List<SoftwareInfo> GetAllClientInstalledApps()
        {
            List<SoftwareInfo> lstSoftInfos = new List<SoftwareInfo>();

            RegistryKey[] keys = new RegistryKey[] {
             // search in: CurrentUser
             Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall"),
            // search in: LocalMachine_32
              Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall"),
            // search in: LocalMachine_64
              Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall")
            };


            foreach (var keyItem in keys)
            {
                var key = keyItem;
                if (null != key)
                {
                    foreach (String keyName in key.GetSubKeyNames())
                    {

                        var modInfo = SystemAppExtension.InitSoftInfoModelFromRegistryKey(ref key, keyName);
                        if (null != modInfo)
                        {
                            lstSoftInfos.Add(modInfo);
                        }
                    }
                }
            }


            return lstSoftInfos;



        }




        /// <summary>
        /// 刷新托盘通知区域
        /// </summary>
        public static void RefreshTrayArea()
        {
            IntPtr systemTrayContainerHandle = FindWindow("Shell_TrayWnd", null);//任务栏窗口  
            IntPtr systemTrayHandle = FindWindowEx(systemTrayContainerHandle, IntPtr.Zero, "TrayNotifyWnd", null);//任务栏右边托盘图标+时间区
            IntPtr sysPagerHandle = FindWindowEx(systemTrayHandle, IntPtr.Zero, "SysPager", null); //不同系统可能有可能没有这层 


            IntPtr notificationAreaHandle = IntPtr.Zero;
            if (sysPagerHandle != IntPtr.Zero)
            {
                notificationAreaHandle = FindWindowEx(sysPagerHandle, IntPtr.Zero, "ToolbarWindow32", null);
            }
            else
            {
                notificationAreaHandle = FindWindowEx(systemTrayHandle, IntPtr.Zero, "ToolbarWindow32", null);
            }

            if (notificationAreaHandle == IntPtr.Zero)
            {
                notificationAreaHandle = FindWindowEx(sysPagerHandle, IntPtr.Zero, "ToolbarWindow32",
                    "User Promoted Notification Area");
                IntPtr notifyIconOverflowWindowHandle = FindWindow("NotifyIconOverflowWindow", null);
                IntPtr overflowNotificationAreaHandle = FindWindowEx(notifyIconOverflowWindowHandle, IntPtr.Zero,
                    "ToolbarWindow32", "Overflow Notification Area");
                RefreshTrayArea(overflowNotificationAreaHandle);
            }
            RefreshTrayArea(notificationAreaHandle);
        }

        private static void RefreshTrayArea(IntPtr windowHandle)
        {
            const uint wmMousemove = 0x0200;
            RECT rect;
            GetClientRect(windowHandle, out rect);
            for (var x = 0; x < rect.right; x += 5)
                for (var y = 0; y < rect.bottom; y += 5)
                    SendMessage(windowHandle, wmMousemove, 0, (y << 16) + x);
        }

    }

}
