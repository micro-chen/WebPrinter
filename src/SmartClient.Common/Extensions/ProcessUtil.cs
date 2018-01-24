using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace SmartClient.Common.Extensions
{
    public class ProcessUtil
    {

        #region ~declare struct,enum  
        [StructLayout(LayoutKind.Sequential)]
        public struct STARTUPINFO
        {
            public Int32 cb;
            public string lpReserved;
            public string lpDesktop;
            public string lpTitle;
            public Int32 dwX;
            public Int32 dwY;
            public Int32 dwXSize;
            public Int32 dwXCountChars;
            public Int32 dwYCountChars;
            public Int32 dwFillAttribute;
            public Int32 dwFlags;
            public Int16 wShowWindow;
            public Int16 cbReserved2;
            public IntPtr lpReserved2;
            public IntPtr hStdInput;
            public IntPtr hStdOutput;
            public IntPtr hStdError;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PROCESS_INFORMATION
        {
            public IntPtr hProcess;
            public IntPtr hThread;
            public Int32 dwProcessID;
            public Int32 dwThreadID;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SECURITY_ATTRIBUTES
        {
            public Int32 Length;
            public IntPtr lpSecurityDescriptor;
            public bool bInheritHandle;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct WTS_SESSION_INFO
        {
            public int SessionId;
            public IntPtr pWinStationName;
            public WTS_CONNECTSTATE_CLASS State;
        }

        public enum SECURITY_IMPERSONATION_LEVEL
        {
            SecurityAnonymous,
            SecurityIdentification,
            SecurityImpersonation,
            SecurityDelegation
        }
        public enum TOKEN_TYPE
        {
            TokenPrimary = 1,
            TokenImpersonation
        }
        public enum WTS_CONNECTSTATE_CLASS
        {
            WTSActive,
            WTSConnected,
            WTSConnectQuery,
            WTSShadow,
            WTSDisconnected,
            WTSIdle,
            WTSListen,
            WTSReset,
            WTSDown,
            WTSInit
        }
        public enum SESSION_TYPE
        {
            SessionFromActiveConsoleSessionId = 0,
            SessionFromEnumerateSessions,
            SessionFromProcessExplorerSession
        }
        #endregion

        #region ~delacre const value  
        public const int GENERIC_ALL_ACCESS = 0x10000000;
        public const int CREATE_NO_WINDOW = 0x08000000;
        public const int CREATE_UNICODE_ENVIRONMENT = 0x00000400;

        public const Int32 STANDARD_RIGHTS_REQUIRED = 0x000F0000;
        public const Int32 STANDARD_RIGHTS_READ = 0x00020000;
        public const Int32 TOKEN_ASSIGN_PRIMARY = 0x0001;
        public const Int32 TOKEN_DUPLICATE = 0x0002;
        public const Int32 TOKEN_IMPERSONATE = 0x0004;
        public const Int32 TOKEN_QUERY = 0x0008;
        public const Int32 TOKEN_QUERY_SOURCE = 0x0010;
        public const Int32 TOKEN_ADJUST_PRIVILEGES = 0x0020;
        public const Int32 TOKEN_ADJUST_GROUPS = 0x0040;
        public const Int32 TOKEN_ADJUST_DEFAULT = 0x0080;
        public const Int32 TOKEN_ADJUST_SESSIONID = 0x0100;
        public const Int32 TOKEN_READ = (STANDARD_RIGHTS_READ | TOKEN_QUERY);
        public const Int32 TOKEN_ALL_ACCESS = (STANDARD_RIGHTS_REQUIRED | TOKEN_ASSIGN_PRIMARY |
            TOKEN_DUPLICATE | TOKEN_IMPERSONATE | TOKEN_QUERY | TOKEN_QUERY_SOURCE |
            TOKEN_ADJUST_PRIVILEGES | TOKEN_ADJUST_GROUPS | TOKEN_ADJUST_DEFAULT |
            TOKEN_ADJUST_SESSIONID);

        public const int MB_ABORTRETRYIGNORE = 0x0002;
        public const int MB_CANCELTRYCONTINUE = 0x0006;
        public const int MB_HELP = 0x4000;
        public const int MB_OK = 0x0000;
        public const int MB_OKCANCEL = 0x0001;
        public const int MB_RETRYCANCEL = 0x0005;
        public const int MB_YESNO = 0x0004;
        public const int MB_YESNOCANCEL = 0x0003;

        #endregion

        #region ~import functions with Win32 API from win32 system dll  
        [DllImport("wtsapi32.dll", SetLastError = true)]
        public static extern bool WTSSendMessage(
            IntPtr hServer,
            int SessionId,
            String pTitle,
            int TitleLength,
            String pMessage,
            int MessageLength,
            int Style,
            int Timeout,
            out int pResponse,
            bool bWait);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int WTSGetActiveConsoleSessionId();

        [DllImport("wtsapi32.dll", SetLastError = true)]
        public static extern bool WTSQueryUserToken(Int32 sessionId, out IntPtr Token);

        [DllImport("userenv.dll", SetLastError = true)]
        static extern bool CreateEnvironmentBlock(out IntPtr lpEnvironment, IntPtr hToken, bool bInherit);

        [DllImport("userenv.dll", SetLastError = true)]
        static extern bool DestroyEnvironmentBlock(IntPtr lpEnvironment);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool CloseHandle(IntPtr handle);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool CreateProcessAsUser(
            IntPtr hToken,
            string lpApplicationName,
            string lpCommandLine,
            ref SECURITY_ATTRIBUTES lpProcessAttributes,
            ref SECURITY_ATTRIBUTES lpThreadAttributes,
            bool bInheritHandle,
            Int32 dwCreationFlags,
            IntPtr lpEnvrionment,
            string lpCurrentDirectory,
            ref STARTUPINFO lpStartupInfo,
            ref PROCESS_INFORMATION lpProcessInformation);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool DuplicateTokenEx(
            IntPtr hExistingToken,
            Int32 dwDesiredAccess,
            ref SECURITY_ATTRIBUTES lpThreadAttributes,
            Int32 ImpersonationLevel,
            Int32 dwTokenType,
            ref IntPtr phNewToken);

        [DllImport("wtsapi32.dll", SetLastError = true)]
        public static extern void WTSFreeMemory(IntPtr pMemory);


        [DllImport("wtsapi32.dll", SetLastError = true)]
        public static extern bool WTSEnumerateSessions(
            IntPtr hServer,
            int Reserved,
            int Version,
            ref IntPtr ppSessionInfo,//WTS_SESSION_INFO PWTS_SESSION_INFO *ppSessionInfo,  
            ref int pCount
            );
        #endregion

        /// <summary>  
        /// Show a MessageBox on the active UI desktop  
        /// </summary>  
        /// <param name="title">title of the MessageBox</param>  
        /// <param name="message">message to show to the user</param>  
        /// <param name="bWait">indicates if wait for some time by parameter timeout  </param>  
        /// <returns>success to return true</returns>  
        public static bool SendMessageBoxToRemoteDesktop(string title, string message, bool bWait)
        {
            int pResponse = 0;
            IntPtr WTS_CURRENT_SERVER_HANDLE = IntPtr.Zero;
            return WTSSendMessage(WTS_CURRENT_SERVER_HANDLE, GetSessionIdFromActiveConsoleSessionId(), title, title.Length, message, message.Length, MB_OK, 0, out pResponse, bWait);
        }

        /// <summary>  
        /// Show a MessageBox on the active UI desktop  
        /// </summary>  
        /// <param name="title">title of the MessageBox</param>  
        /// <param name="message">message to show to the user</param>  
        /// <param name="button_style">can be a combination of MessageBoxButtons and MessageBoxIcon, need to convert it to int</param>  
        /// <param name="timeout">timeout to determine when to return this function call, 0 means wait until the user response the MessageBox</param>  
        /// <param name="bWait">indicates if wait for some time by parameter timeout  </param>  
        /// <returns>success to return true</returns>  
        public static bool SendMessageBoxToRemoteDesktop(string title, string message, int button_style, int timeout, bool bWait)
        {
            int pResponse = 0;
            IntPtr WTS_CURRENT_SERVER_HANDLE = IntPtr.Zero;
            return WTSSendMessage(WTS_CURRENT_SERVER_HANDLE, GetSessionIdFromActiveConsoleSessionId(), title, title.Length, message, message.Length, button_style, timeout, out pResponse, bWait);
        }

        /// <summary>  
        /// Show a MessageBox on the active UI desktop  
        /// </summary>  
        /// <param name="title">title of the MessageBox</param>  
        /// <param name="message">message to show to the user</param>  
        /// <param name="button_style">can be a combination of MessageBoxButtons and MessageBoxIcon, need to convert it to int</param>  
        /// <param name="timeout">timeout to determine when to return this function call, 0 means wait until the user response the MessageBox</param>  
        /// <param name="pResponse">pointer to receive the button result which clicked by user</param>  
        /// <param name="bWait">indicates if wait for some time by parameter timeout  </param>  
        /// <returns>success to return true</returns>  
        public static bool SendMessageBoxToRemoteDesktop(string title, string message, int button_style, int timeout, out int pResponse, bool bWait)
        {
            IntPtr WTS_CURRENT_SERVER_HANDLE = IntPtr.Zero;
            return WTSSendMessage(WTS_CURRENT_SERVER_HANDLE, GetSessionIdFromActiveConsoleSessionId(), title, title.Length, message, message.Length, button_style, timeout, out pResponse, bWait);
        }

        public static int GetSessionIdFromActiveConsoleSessionId()
        {
            int dwSessionID = WTSGetActiveConsoleSessionId();
            return dwSessionID;
        }

        public static int GetSessionIdFromEnumerateSessions()
        {
            IntPtr WTS_CURRENT_SERVER_HANDLE = IntPtr.Zero;
            int dwSessionId = 0;
            IntPtr pSessionInfo = IntPtr.Zero;
            int dwCount = 0;

            WTSEnumerateSessions(WTS_CURRENT_SERVER_HANDLE, 0, 1,
                                 ref pSessionInfo, ref dwCount);

            Int32 dataSize = Marshal.SizeOf(typeof(WTS_SESSION_INFO));
            Int32 current = (int)pSessionInfo;
            for (int i = 0; i < dwCount; i++)
            {
                WTS_SESSION_INFO si = (WTS_SESSION_INFO)Marshal.PtrToStructure(
                    (System.IntPtr)current, typeof(WTS_SESSION_INFO));
                if (WTS_CONNECTSTATE_CLASS.WTSActive == si.State)
                {
                    dwSessionId = si.SessionId;
                    break;
                }

                current += dataSize;
            }

            WTSFreeMemory(pSessionInfo);
            return dwSessionId;
        }

        public static int GetSessionIdFromExplorerSessionId()
        {
            int dwSessionId = 0;
            Process[] process_array = Process.GetProcessesByName("explorer");
            if (process_array.Length > 0)
            {
                dwSessionId = process_array[0].SessionId;
            }

            return dwSessionId;

        }

        public static Process CreateProcessAsUser(string filename, string args, SESSION_TYPE session_method= SESSION_TYPE.SessionFromEnumerateSessions)
        {
            IntPtr hToken = IntPtr.Zero;//WindowsIdentity.GetCurrent().Token;  
            int dwSessionId = 0;
            IntPtr hDupedToken = IntPtr.Zero;
            Int32 dwCreationFlags = 0;
            PROCESS_INFORMATION pi = new PROCESS_INFORMATION();
            SECURITY_ATTRIBUTES sa = new SECURITY_ATTRIBUTES();
            sa.Length = Marshal.SizeOf(sa);
            STARTUPINFO si = new STARTUPINFO();
            si.cb = Marshal.SizeOf(si);
            si.lpDesktop = "";
            IntPtr lpEnvironment = IntPtr.Zero;
            string full_filepath = Path.GetFullPath(filename);
            string working_Dir = Path.GetDirectoryName(full_filepath);

            try
            {
                #region ~ get sessionid  
                switch (session_method)
                {
                    case SESSION_TYPE.SessionFromActiveConsoleSessionId:
                        dwSessionId = GetSessionIdFromActiveConsoleSessionId();
                        break;

                    case SESSION_TYPE.SessionFromEnumerateSessions:
                        dwSessionId = GetSessionIdFromEnumerateSessions();
                        break;

                    case SESSION_TYPE.SessionFromProcessExplorerSession:
                        dwSessionId = GetSessionIdFromExplorerSessionId();
                        break;

                    default:
                        dwSessionId = GetSessionIdFromActiveConsoleSessionId();
                        break;
                }
                #endregion
                #region ~ retrieve Token from a specified SessionId  
                bool bResult = WTSQueryUserToken(dwSessionId, out hToken);
                if (!bResult)
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }
                #endregion
                #region ~ Duplicate from the specified Token  
                if (!DuplicateTokenEx(
                        hToken,
                        GENERIC_ALL_ACCESS,
                        ref sa,
                        (int)SECURITY_IMPERSONATION_LEVEL.SecurityIdentification,
                        (int)TOKEN_TYPE.TokenPrimary,
                        ref hDupedToken
                    ))
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                #endregion
                #region ~ Create a Environment Block from specifid Token  
                bool result = CreateEnvironmentBlock(out lpEnvironment, hDupedToken, false);
                if (!result)
                {
                    lpEnvironment = IntPtr.Zero;    //if fail, reset it to Zero  
                }
                else
                {
                    dwCreationFlags = CREATE_UNICODE_ENVIRONMENT;   //if success, set the CreationsFlags to CREATE_UNICODE_ENVIRONMENT, then pass it into CreateProcessAsUser  
                }
                #endregion
                #region ~ Create a Process with Specified Token  
                if (!CreateProcessAsUser(
                    hDupedToken,
                    full_filepath,
                    string.Format("\"{0}\" {1}", filename.Replace("\"", ""), args),
                    ref sa,
                    ref sa,
                    false,
                    dwCreationFlags,
                    lpEnvironment,
                    working_Dir,
                    ref si,
                    ref pi
                    ))
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                #endregion
                #region ~ Destroy the Environment Block which is Created by CreateEnvironment  
                DestroyEnvironmentBlock(lpEnvironment);
                #endregion

                return Process.GetProcessById(pi.dwProcessID);
            }
            catch (Win32Exception e)
            {
                #region ~handle win32 exception  
                int pResponse = 0;
                string errMsg =
                    "NativeErrorCode:\t" + e.NativeErrorCode
                    + "\n\nSource:  " + e.Source
                    + "\n\nMessage:  " + e.Message
                    + "\n\nStackTrace:  " + e.StackTrace;

                SendMessageBoxToRemoteDesktop("Win32 Exception!!", errMsg, MB_OK, 0, out pResponse, false); //send the error message to the remote desktop  
                return null;
                #endregion
            }
            finally
            {
                #region ~ release hanlde  
                if (pi.hProcess != IntPtr.Zero)
                    CloseHandle(pi.hProcess);
                if (pi.hThread != IntPtr.Zero)
                    CloseHandle(pi.hThread);
                if (hDupedToken != IntPtr.Zero)
                    CloseHandle(hDupedToken);
                #endregion
            }
        }
    }
}
