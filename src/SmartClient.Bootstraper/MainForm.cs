
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Owin.Hosting;
using Newtonsoft.Json;
using System.Net.Http;
using System.Timers;
using System.Windows.Automation;

using SmartClient.Web;
using SmartClient.Common;

namespace SmartClient.Bootstraper
{
    /// <summary>
    /// 程序主窗体
    /// </summary>
    public partial class MainForm : Form
    {
        /// <summary>
        /// 内置的Server
        /// </summary>
        public IDisposable AppServer { get; private set; }

        private static System.Timers.Timer timerFoKeepAlive = null;

        /// <summary>
        /// 频率进行定时的请求IIS /Mono /Owin  防止休眠
        /// </summary>
        private const int TickFrequencyFoKeepAlive = 15 * 1000;

        public MainForm()
        {
            InitializeComponent();

            this.Load += MainForm_Load;
            this.FormClosed += MainForm_FormClosed;
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (null != this.AppServer)
            {
                this.AppServer.Dispose();//释放Server
            }
        }


        /// <summary>
        /// 程序加载启动的时候事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_Load(object sender, EventArgs e)
        {
            //1 检测当前启动进程的用户是否在管理员权限组
            var isInAdminGroup = this.IsUserInAdminGroup();
            if (!isInAdminGroup)
            {
                MessageBox.Show("您所在的用户组没有管理员权限！请联系本机系统管理员授权！", "用户权限检测", MessageBoxButtons.OK, MessageBoxIcon.Error);
                System.Environment.Exit(0);//终止程序进程 及其子进程
                return;
            }

            //2-1 如果是在XP 环境 那么弹出提示框（必须弹出提示，Web打开本地的程序 必须让用户知道启动了exe程序）
            // 2-2 Get and display the process elevation information (IsProcessElevated) 
            // and integrity level (GetProcessIntegrityLevel). The information is not 
            // available on operating systems prior to Windows Vista.
            if (Environment.OSVersion.Version.Major >= 6)
            {
                // Running Windows Vista or later (major version >= 6). UAC的时候 会提示框

                try
                {
                    //判断是否需要UAC提升权限
                    // Elevate the process if it is not run as administrator.
                    if (!IsRunAsAdmin())
                    {
                        // Launch itself as administrator
                        ProcessStartInfo proc = new ProcessStartInfo();
                        proc.UseShellExecute = true;
                        proc.WorkingDirectory = Environment.CurrentDirectory;
                        proc.FileName = Application.ExecutablePath;
                        proc.Verb = "runas";

                        try
                        {
                            //win7弹层
                            PopUACWindowTopLayerAsync();

                            Process.Start(proc);

                        }
                        catch
                        { }
                        finally
                        {
                            // The user refused the elevation.用户拒绝授权UAC 那么程序退出
                            System.Environment.Exit(0);//终止程序进程 及其子进程
                        }
                        return;//执行完毕上述代码后 必须return  否则会出现下面的检测多个实例的异常
                    }

                }
                catch (Exception ex)
                {

                    MessageBox.Show(ex.Message, "打开程序授权UAC失败！",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }


            }
            else
            {
                //XP ---弹出确认框
                var cfmResult = MessageBox.Show("是否确定开启智能打印客户端工作台程序?", "用户启动控制！",
                        MessageBoxButtons.OKCancel, MessageBoxIcon.Information);

                if (cfmResult != DialogResult.OK)
                {
                    System.Environment.Exit(0);
                    return;
                }
            }


            var isRunning = HideOnStartupApplicationContext.IsCurrentAppProcessHasRunning();
            if (isRunning)
            {
                MessageBox.Show("程序已经正在运行中！不允许运行多个实例！", "程序状态检测", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                System.Environment.Exit(0);//终止程序进程 及其子进程
                return;
            }


            //MessageBox.Show("okokokoko");
            //3 通过了检测授权 开启Owin环境
            try
            {
                RunOwinServer();
            }
            catch (Exception ex)
            {
                //非正常启动 http监听 那么退出 并记录日志
                Logger.WriteException(ex);
                System.Environment.Exit(0);//终止程序进程 及其子进程
            }
         
        }

        private void RunOwinServer()
        {
            try
            {
                AppServer = WebApp.Start<Startup>(GlobalConfig.BaseBindingAddress);

                Debug.WriteLine("we are ready......press enter to end");

                Task.Factory.StartNew(() =>
                {
                    LoadingOneRequestTest();


                    //启动定时器 开始每间隔一定的事件 发送一次请求 防止进程被回收
                    timerFoKeepAlive = new System.Timers.Timer(TickFrequencyFoKeepAlive);
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



        }


        /// <summary>
        /// 启动后 尝试向程序发送一次HTTP请求，用来进行程序的sgen
        /// </summary>
        static void LoadingOneRequestTest()
        {
            string keepAliveAddr = string.Concat(GlobalConfig.BaseAddress, "/api/WorkBench/KeepAlive");

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
                    Debug.WriteLine("the first init request result is:" + msg.Message);
                    return;
                }

                Debug.WriteLine("Sorry,the first init request error!!!!");




            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());

            }


        }

        #region Helper Functions for Admin Privileges and Elevation Status


        /// <summary>
        /// Win7 / Server2008 请求访问权限的时候 只是附加到了任务栏，触发点击事件 弹出
        /// UISpy.exe
        /// </summary>
        public static void PopUACWindowTopLayerAsync()
        {

            if (Environment.OSVersion.Version.Major == 6 && Environment.OSVersion.Version.Minor == 1)
            {
                //找到Desktop  
                AutomationElement desktop = AutomationElement.RootElement;
                //找到任务栏
                AutomationElement taskBar = desktop.FindFirst(
                    TreeScope.Subtree,
                    new AndCondition(
                        new PropertyCondition(AutomationElement.ClassNameProperty, "MSTaskListWClass"),
                        new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.ToolBar)
                        )
                  );




                //启用轮询
                //目标项
                AutomationElement tagetElement = null;

                System.Timers.Timer tm = new System.Timers.Timer(10);
                tm.Elapsed += (sender, e) =>
                {

                    //任务栏中的子项
                    var barChilds = taskBar.FindAll(TreeScope.Children,
                        new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Button));


                    for (int i = barChilds.Count - 1; i >= 0; i--)
                    {
                        var item = barChilds[i];
                        string name = item.Current.Name;
                        if (name.Contains(HideOnStartupApplicationContext.AppName) && name.Contains("正在请求您的许可"))
                        {

                            tagetElement = item;

                            var invokePattern = tagetElement.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;
                            invokePattern.Invoke();


                            tm.Stop();
                            tm.Dispose();//销毁定时器
                            break;


                        }

                    }

                };

                tm.Start();







            }
        }

        /// <summary>
        /// The function checks whether the primary access token of the process belongs 
        /// to user account that is a member of the local Administrators group, even if 
        /// it currently is not elevated.
        /// </summary>
        /// <returns>
        /// Returns true if the primary access token of the process belongs to user 
        /// account that is a member of the local Administrators group. Returns false 
        /// if the token does not.
        /// </returns>
        /// <exception cref="System.ComponentModel.Win32Exception">
        /// When any native Windows API call fails, the function throws a Win32Exception 
        /// with the last error code.
        /// </exception>
        internal bool IsUserInAdminGroup()
        {
            bool fInAdminGroup = false;
            SafeTokenHandle hToken = null;
            SafeTokenHandle hTokenToCheck = null;
            IntPtr pElevationType = IntPtr.Zero;
            IntPtr pLinkedToken = IntPtr.Zero;
            int cbSize = 0;

            try
            {
                // Open the access token of the current process for query and duplicate.
                if (!NativeMethodsUti.OpenProcessToken(Process.GetCurrentProcess().Handle,
                    NativeMethodsUti.TOKEN_QUERY | NativeMethodsUti.TOKEN_DUPLICATE, out hToken))
                {
                    throw new Win32Exception();
                }

                // Determine whether system is running Windows Vista or later operating 
                // systems (major version >= 6) because they support linked tokens, but 
                // previous versions (major version < 6) do not.
                if (Environment.OSVersion.Version.Major >= 6)
                {
                    // Running Windows Vista or later (major version >= 6). 
                    // Determine token type: limited, elevated, or default. 

                    // Allocate a buffer for the elevation type information.
                    cbSize = sizeof(TOKEN_ELEVATION_TYPE);
                    pElevationType = Marshal.AllocHGlobal(cbSize);
                    if (pElevationType == IntPtr.Zero)
                    {
                        throw new Win32Exception();
                    }

                    // Retrieve token elevation type information.
                    if (!NativeMethodsUti.GetTokenInformation(hToken,
                        TOKEN_INFORMATION_CLASS.TokenElevationType, pElevationType,
                        cbSize, out cbSize))
                    {
                        throw new Win32Exception();
                    }

                    // Marshal the TOKEN_ELEVATION_TYPE enum from native to .NET.
                    TOKEN_ELEVATION_TYPE elevType = (TOKEN_ELEVATION_TYPE)
                        Marshal.ReadInt32(pElevationType);

                    // If limited, get the linked elevated token for further check.
                    if (elevType == TOKEN_ELEVATION_TYPE.TokenElevationTypeLimited)
                    {
                        // Allocate a buffer for the linked token.
                        cbSize = IntPtr.Size;
                        pLinkedToken = Marshal.AllocHGlobal(cbSize);
                        if (pLinkedToken == IntPtr.Zero)
                        {
                            throw new Win32Exception();
                        }

                        // Get the linked token.
                        if (!NativeMethodsUti.GetTokenInformation(hToken,
                            TOKEN_INFORMATION_CLASS.TokenLinkedToken, pLinkedToken,
                            cbSize, out cbSize))
                        {
                            throw new Win32Exception();
                        }

                        // Marshal the linked token value from native to .NET.
                        IntPtr hLinkedToken = Marshal.ReadIntPtr(pLinkedToken);
                        hTokenToCheck = new SafeTokenHandle(hLinkedToken);
                    }
                }

                // CheckTokenMembership requires an impersonation token. If we just got 
                // a linked token, it already is an impersonation token.  If we did not 
                // get a linked token, duplicate the original into an impersonation 
                // token for CheckTokenMembership.
                if (hTokenToCheck == null)
                {
                    if (!NativeMethodsUti.DuplicateToken(hToken,
                        SECURITY_IMPERSONATION_LEVEL.SecurityIdentification,
                        out hTokenToCheck))
                    {
                        throw new Win32Exception();
                    }
                }

                // Check if the token to be checked contains admin SID.
                WindowsIdentity id = new WindowsIdentity(hTokenToCheck.DangerousGetHandle());
                WindowsPrincipal principal = new WindowsPrincipal(id);
                fInAdminGroup = principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            finally
            {
                // Centralized cleanup for all allocated resources. 
                if (hToken != null)
                {
                    hToken.Close();
                    hToken = null;
                }
                if (hTokenToCheck != null)
                {
                    hTokenToCheck.Close();
                    hTokenToCheck = null;
                }
                if (pElevationType != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(pElevationType);
                    pElevationType = IntPtr.Zero;
                }
                if (pLinkedToken != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(pLinkedToken);
                    pLinkedToken = IntPtr.Zero;
                }
            }

            return fInAdminGroup;
        }


        /// <summary>
        /// The function checks whether the current process is run as administrator.
        /// In other words, it dictates whether the primary access token of the 
        /// process belongs to user account that is a member of the local 
        /// Administrators group and it is elevated.
        /// </summary>
        /// <returns>
        /// Returns true if the primary access token of the process belongs to user 
        /// account that is a member of the local Administrators group and it is 
        /// elevated. Returns false if the token does not.
        /// </returns>
        internal bool IsRunAsAdmin()
        {
            WindowsIdentity id = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(id);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }


        /// <summary>
        /// The function gets the elevation information of the current process. It 
        /// dictates whether the process is elevated or not. Token elevation is only 
        /// available on Windows Vista and newer operating systems, thus 
        /// IsProcessElevated throws a C++ exception if it is called on systems prior 
        /// to Windows Vista. It is not appropriate to use this function to determine 
        /// whether a process is run as administartor.
        /// </summary>
        /// <returns>
        /// Returns true if the process is elevated. Returns false if it is not.
        /// </returns>
        /// <exception cref="System.ComponentModel.Win32Exception">
        /// When any native Windows API call fails, the function throws a Win32Exception 
        /// with the last error code.
        /// </exception>
        /// <remarks>
        /// TOKEN_INFORMATION_CLASS provides TokenElevationType to check the elevation 
        /// type (TokenElevationTypeDefault / TokenElevationTypeLimited / 
        /// TokenElevationTypeFull) of the process. It is different from TokenElevation 
        /// in that, when UAC is turned off, elevation type always returns 
        /// TokenElevationTypeDefault even though the process is elevated (Integrity 
        /// Level == High). In other words, it is not safe to say if the process is 
        /// elevated based on elevation type. Instead, we should use TokenElevation. 
        /// </remarks>
        internal bool IsProcessElevated()
        {
            bool fIsElevated = false;
            SafeTokenHandle hToken = null;
            int cbTokenElevation = 0;
            IntPtr pTokenElevation = IntPtr.Zero;

            try
            {
                // Open the access token of the current process with TOKEN_QUERY.
                if (!NativeMethodsUti.OpenProcessToken(Process.GetCurrentProcess().Handle,
                    NativeMethodsUti.TOKEN_QUERY, out hToken))
                {
                    throw new Win32Exception();
                }

                // Allocate a buffer for the elevation information.
                cbTokenElevation = Marshal.SizeOf(typeof(TOKEN_ELEVATION));
                pTokenElevation = Marshal.AllocHGlobal(cbTokenElevation);
                if (pTokenElevation == IntPtr.Zero)
                {
                    throw new Win32Exception();
                }

                // Retrieve token elevation information.
                if (!NativeMethodsUti.GetTokenInformation(hToken,
                    TOKEN_INFORMATION_CLASS.TokenElevation, pTokenElevation,
                    cbTokenElevation, out cbTokenElevation))
                {
                    // When the process is run on operating systems prior to Windows 
                    // Vista, GetTokenInformation returns false with the error code 
                    // ERROR_INVALID_PARAMETER because TokenElevation is not supported 
                    // on those operating systems.
                    throw new Win32Exception();
                }

                // Marshal the TOKEN_ELEVATION struct from native to .NET object.
                TOKEN_ELEVATION elevation = (TOKEN_ELEVATION)Marshal.PtrToStructure(
                    pTokenElevation, typeof(TOKEN_ELEVATION));

                // TOKEN_ELEVATION.TokenIsElevated is a non-zero value if the token 
                // has elevated privileges; otherwise, a zero value.
                fIsElevated = (elevation.TokenIsElevated != 0);
            }
            finally
            {
                // Centralized cleanup for all allocated resources. 
                if (hToken != null)
                {
                    hToken.Close();
                    hToken = null;
                }
                if (pTokenElevation != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(pTokenElevation);
                    pTokenElevation = IntPtr.Zero;
                    cbTokenElevation = 0;
                }
            }

            return fIsElevated;
        }


        /// <summary>
        /// The function gets the integrity level of the current process. Integrity 
        /// level is only available on Windows Vista and newer operating systems, thus 
        /// GetProcessIntegrityLevel throws a C++ exception if it is called on systems 
        /// prior to Windows Vista.
        /// </summary>
        /// <returns>
        /// Returns the integrity level of the current process. It is usually one of 
        /// these values:
        /// 
        ///    SECURITY_MANDATORY_UNTRUSTED_RID - means untrusted level. It is used 
        ///    by processes started by the Anonymous group. Blocks most write access.
        ///    (SID: S-1-16-0x0)
        ///    
        ///    SECURITY_MANDATORY_LOW_RID - means low integrity level. It is used by
        ///    Protected Mode Internet Explorer. Blocks write acess to most objects 
        ///    (such as files and registry keys) on the system. (SID: S-1-16-0x1000)
        /// 
        ///    SECURITY_MANDATORY_MEDIUM_RID - means medium integrity level. It is 
        ///    used by normal applications being launched while UAC is enabled. 
        ///    (SID: S-1-16-0x2000)
        ///    
        ///    SECURITY_MANDATORY_HIGH_RID - means high integrity level. It is used 
        ///    by administrative applications launched through elevation when UAC is 
        ///    enabled, or normal applications if UAC is disabled and the user is an 
        ///    administrator. (SID: S-1-16-0x3000)
        ///    
        ///    SECURITY_MANDATORY_SYSTEM_RID - means system integrity level. It is 
        ///    used by services and other system-level applications (such as Wininit, 
        ///    Winlogon, Smss, etc.)  (SID: S-1-16-0x4000)
        /// 
        /// </returns>
        /// <exception cref="System.ComponentModel.Win32Exception">
        /// When any native Windows API call fails, the function throws a Win32Exception 
        /// with the last error code.
        /// </exception>
        internal int GetProcessIntegrityLevel()
        {
            int IL = -1;
            SafeTokenHandle hToken = null;
            int cbTokenIL = 0;
            IntPtr pTokenIL = IntPtr.Zero;

            try
            {
                // Open the access token of the current process with TOKEN_QUERY.
                if (!NativeMethodsUti.OpenProcessToken(Process.GetCurrentProcess().Handle,
                    NativeMethodsUti.TOKEN_QUERY, out hToken))
                {
                    throw new Win32Exception();
                }

                // Then we must query the size of the integrity level information 
                // associated with the token. Note that we expect GetTokenInformation 
                // to return false with the ERROR_INSUFFICIENT_BUFFER error code 
                // because we've given it a null buffer. On exit cbTokenIL will tell 
                // the size of the group information.
                if (!NativeMethodsUti.GetTokenInformation(hToken,
                    TOKEN_INFORMATION_CLASS.TokenIntegrityLevel, IntPtr.Zero, 0,
                    out cbTokenIL))
                {
                    int error = Marshal.GetLastWin32Error();
                    if (error != NativeMethodsUti.ERROR_INSUFFICIENT_BUFFER)
                    {
                        // When the process is run on operating systems prior to 
                        // Windows Vista, GetTokenInformation returns false with the 
                        // ERROR_INVALID_PARAMETER error code because 
                        // TokenIntegrityLevel is not supported on those OS's.
                        throw new Win32Exception(error);
                    }
                }

                // Now we allocate a buffer for the integrity level information.
                pTokenIL = Marshal.AllocHGlobal(cbTokenIL);
                if (pTokenIL == IntPtr.Zero)
                {
                    throw new Win32Exception();
                }

                // Now we ask for the integrity level information again. This may fail 
                // if an administrator has added this account to an additional group 
                // between our first call to GetTokenInformation and this one.
                if (!NativeMethodsUti.GetTokenInformation(hToken,
                    TOKEN_INFORMATION_CLASS.TokenIntegrityLevel, pTokenIL, cbTokenIL,
                    out cbTokenIL))
                {
                    throw new Win32Exception();
                }

                // Marshal the TOKEN_MANDATORY_LABEL struct from native to .NET object.
                TOKEN_MANDATORY_LABEL tokenIL = (TOKEN_MANDATORY_LABEL)
                    Marshal.PtrToStructure(pTokenIL, typeof(TOKEN_MANDATORY_LABEL));

                // Integrity Level SIDs are in the form of S-1-16-0xXXXX. (e.g. 
                // S-1-16-0x1000 stands for low integrity level SID). There is one 
                // and only one subauthority.
                IntPtr pIL = NativeMethodsUti.GetSidSubAuthority(tokenIL.Label.Sid, 0);
                IL = Marshal.ReadInt32(pIL);
            }
            finally
            {
                // Centralized cleanup for all allocated resources. 
                if (hToken != null)
                {
                    hToken.Close();
                    hToken = null;
                }
                if (pTokenIL != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(pTokenIL);
                    pTokenIL = IntPtr.Zero;
                    cbTokenIL = 0;
                }
            }

            return IL;
        }

        #endregion

    }
}
