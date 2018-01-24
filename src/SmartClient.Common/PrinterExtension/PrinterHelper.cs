using System;
using System.Text;
using System.Runtime.InteropServices;
using System.Security;
using System.ComponentModel;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace SmartClient.Common.PrinterExtension
{
    /// <summary>
    /// Origin: http://blog.csdn.net/csui2008/article/details/5718461
    /// Modified and a little tested by Huan-Lin Tsai. May-29-2013.
    /// </summary>
    public static class PrinterHelper
    {
        #region "Private Variables"
        private static int lastError;
        private static int nRet;   //long 
        private static int intError;
        private static System.Int32 nJunk;

        #endregion

        #region "API Define"


        /// <summary>
        /// 设置默认打印机
        /// The SetDefaultPrinter function sets the printer name of the default printer for the current user on the local computer
        ///If this parameter is NULL or an empty string, that is, "", SetDefaultPrinter will select a default printer from one of the installed printers. If a default printer already exists, calling SetDefaultPrinter with a NULL or an empty string in this parameter might change the default printer.
        /// MSDN:https://msdn.microsoft.com/en-us/library/dd162971.aspx
        /// </summary>
        /// <param name="printerName">pszPrinter [in] A pointer to a null-terminated string containing the default printer name.For a remote printer connection, the name format is \\server\printername.For a local printer, the name format is printername.</param>
        /// <returns></returns>
        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool SetDefaultPrinter(string printerName);

        /// <summary>
        /// The FlushPrinter function sends a buffer to the printer in order to clear it from a transient state
        /// This is a blocking or synchronous function and might not return immediately. How quickly this function returns depends on run-time factors such as network status, print server configuration, and printer driver implementation—factors that are difficult to predict when writing an application. Calling this function from a thread that manages interaction with the user interface could make the application appear to be unresponsive.
        /// FlushPrinter should be called only if WritePrinter failed, leaving the printer in a transient state. For example, the printer could get into a transient state when the job gets aborted and the printer driver has partially sent some raw data to the printer.
        ///FlushPrinter also can specify an idle period during which the print spooler does not schedule any jobs to the corresponding printer port.
        ///MSDN:https://msdn.microsoft.com/zh-cn/library/windows/desktop/dd144818.aspx
        /// </summary>
        /// <param name="hPrinter">hPrinter [in] A handle to the printer object. This should be the same handle that was used, in a prior WritePrinter call, by the printer driver.</param>
        /// <param name="pBuf">pBuf [in] A pointer to an array of bytes that contains the data to be written to the printer.</param>
        /// <param name="cbBuf">cbBuf [in]The size, in bytes, of the array pointed to by pBuf.</param>
        /// <param name="pcWritten">pcWritten [out]A pointer to a value that receives the number of bytes of data that were written to the printer.</param>
        /// <param name="cSleep">cSleep [in]The time, in milliseconds, for which the I/O line to the printer port should be kept idle.</param>
        /// <returns></returns>
        [DllImport("winspool.drv", EntryPoint = "FlushPrinter", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool FlushPrinter(IntPtr hPrinter,
            IntPtr pBuf,
            Int32 cbBuf,
            out Int32 pcWritten,
            Int32 cSleep);


        /// <summary>
        /// The StartDocPrinter function notifies the print spooler that a document is to be spooled for printing.
        /// The typical sequence for a print job is as follows:
        ///To begin a print job, call StartDocPrinter.
        ///To begin each page, call StartPagePrinter.
        ///To write data to a page, call WritePrinter.
        ///To end each page, call EndPagePrinter.
        ///Repeat 2, 3, and 4 for as many pages as necessary.
        ///To end the print job, call EndDocPrinter.
        ///Note that calling StartPagePrinter and EndPagePrinter may not be necessary, such as if the print data type includes the page information.
        ///When a page in a spooled file exceeds approximately 350 MB, it can fail to print and not send an error message. For example, this can occur when printing large EMF files. The page size limit depends on many factors including the amount of virtual memory available, the amount of memory allocated by calling processes, and the amount of fragmentation in the process heap.
        ///Examples
        ///For a sample program that uses this function, see How To: Print Using the GDI Print API.
        /// MSDN:https://msdn.microsoft.com/en-us/library/windows/desktop/dd145115(v=vs.85).aspx
        /// </summary>
        /// <param name="hPrinter">hPrinter [in]   A handle to the printer.Use the OpenPrinter or AddPrinter function to retrieve a printer handle.</param>
        /// <param name="level">Level [in]The version of the structure to which pDocInfo points.This value must be 1.</param>
        /// <param name="di">pDocInfo [in]  A pointer to a DOC_INFO_1 structure that describes the document to print.</param>
        /// <returns></returns>
        [DllImport("winspool.drv", EntryPoint = "StartDocPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        internal static extern bool StartDocPrinterA(IntPtr hPrinter,
            Int32 level,
            [In] DOCINFOA di);


        [DllImport("winspool.drv", EntryPoint = "StartDocPrinterW", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        internal static extern bool StartDocPrinterW(IntPtr hPrinter,
            Int32 level,
            [In] DOCINFOW di);



        /// <summary>
        /// The EndDocPrinter function ends a print job for the specified printer.
        /// MSDN:https://msdn.microsoft.com/en-us/library/windows/desktop/dd162595(v=vs.85).aspx
        /// </summary>
        /// <param name="hPrinter">hPrinter [in]  Handle to a printer for which the print job should be ended.Use the OpenPrinter or AddPrinter function to retrieve a printer handle.</param>
        /// <returns></returns>
        [DllImport("winspool.drv", EntryPoint = "EndDocPrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        internal static extern bool EndDocPrinter(IntPtr hPrinter);

        /// <summary>
        /// The StartPagePrinter function notifies the spooler that a page is about to be printed on the specified printer.
        /// MSDN:https://msdn.microsoft.com/en-us/library/windows/desktop/dd145117(v=vs.85).aspx
        /// </summary>
        /// <param name="hPrinter">hPrinter [in]   Handle to a printer.Use the OpenPrinter or AddPrinter function to retrieve a printer handle.</param>
        /// <returns></returns>
        [DllImport("winspool.drv", EntryPoint = "StartPagePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        internal static extern bool StartPagePrinter(IntPtr hPrinter);



        /// <summary>
        /// The EndPagePrinter function notifies the print spooler that the application is at the end of a page in a print job.
        /// MSDN:https://msdn.microsoft.com/en-us/library/windows/desktop/dd162597(v=vs.85).aspx
        /// </summary>
        /// <param name="hPrinter">hPrinter [in] Handle to the printer for which the page will be concluded.Use the OpenPrinter or AddPrinter function to retrieve a printer handle.</param>
        /// <returns></returns>
        [DllImport("winspool.drv", EntryPoint = "EndPagePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        internal static extern bool EndPagePrinter(IntPtr hPrinter);



        /// <summary>
        /// The WritePrinter function notifies the print spooler that data should be written to the specified printer.
        /// 
        /// Note  WritePrinter only supports GDI printing and must not be used for XPS printing. If your print job uses the XPS or the OpenXPS print path, then use the XPS Print API. Sending XPS or OpenXPS print jobs to the spooler using WritePrinter is not supported and can result in undetermined results.
        /// </summary>
        /// <param name="hPrinter">A handle to the printer. Use the OpenPrinter or AddPrinter function to retrieve a printer handle.</param>
        /// <param name="pBytes">A pointer to an array of bytes that contains the data that should be written to the printer.</param>
        /// <param name="dwCount">The size, in bytes, of the array.</param>
        /// <param name="dwWritten">[out] A pointer to a value that receives the number of bytes of data that were written to the printer.</param>
        /// <returns></returns>
        [DllImport("winspool.drv", EntryPoint = "WritePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        internal static extern bool WritePrinter(IntPtr hPrinter,
            IntPtr pBytes,
            Int32 dwCount,
            out Int32 dwWritten);

        /// <summary>
        /// 关闭打印机
        /// The ClosePrinter function closes the specified printer object.
        /// MSDN:https://msdn.microsoft.com/en-us/library/windows/desktop/dd183446(v=vs.85).aspx
        /// </summary>
        /// <param name="hPrinter">A handle to the printer object to be closed. This handle is returned by the OpenPrinter or AddPrinter function.</param>
        /// <returns></returns>
        [DllImport("winspool.drv", EntryPoint = "ClosePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool ClosePrinter(IntPtr hPrinter);



        /// <summary>
        /// The DocumentProperties function retrieves or modifies printer initialization information or displays a printer-configuration property sheet for the specified printer.
        /// MSDN:https://msdn.microsoft.com/en-us/library/windows/desktop/dd183576(v=vs.85).aspx
        /// </summary>
        /// <param name="hwnd">A handle to the parent window of the printer-configuration property sheet.</param>
        /// <param name="hPrinter">A handle to a printer object. Use the OpenPrinter or AddPrinter function to retrieve a printer handle.</param>
        /// <param name="pDeviceName">A pointer to a null-terminated string that specifies the name of the device for which the printer-configuration property sheet is displayed.</param>
        /// <param name="pDevModeOutput">[out]A pointer to a DEVMODE structure that receives the printer configuration data specified by the user.</param>
        /// <param name="pDevModeInput">A pointer to a DEVMODE structure that the operating system uses to initialize the property sheet controls. This parameter is only used if the DM_IN_BUFFER flag is set in the fMode parameter.If DM_IN_BUFFER is not set, the operating system uses the printer's default DEVMODE.</param>
        /// <param name="fMode">The operations the function performs. If this parameter is zero, the DocumentProperties function returns the number of bytes required by the printer driver's DEVMODE data structure. Otherwise, use one or more of the following constants to construct a value for this parameter; note, however, that in order to change the print settings, an application must specify at least one input value and one output value.</param>
        /// <returns></returns>
        [DllImport("winspool.drv", EntryPoint = "DocumentPropertiesA", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        internal static extern int DocumentProperties(
            IntPtr hwnd,
            IntPtr hPrinter,
            [MarshalAs(UnmanagedType.LPStr)] string pDeviceName,
            IntPtr pDevModeOutput,
            IntPtr pDevModeInput,
            int fMode
            );



        /// <summary>
        /// The GetPrinter function retrieves information about a specified printer.
        /// MSDN:https://msdn.microsoft.com/en-us/library/windows/desktop/dd144911(v=vs.85).aspx
        /// </summary>
        /// <param name="hPrinter">A handle to the printer for which the function retrieves information. Use the OpenPrinter or AddPrinter function to retrieve a printer handle.</param>
        /// <param name="dwLevel">The level or type of structure that the function stores into the buffer pointed to by pPrinter.This value can be 1, 2, 3, 4, 5, 6, 7, 8 or 9.</param>
        /// <param name="pPrinter">A pointer to a buffer that receives a structure containing information about the specified printer. The buffer must be large enough to receive the structure and any strings or other data to which the structure members point. If the buffer is too small, the pcbNeeded parameter returns the required buffer size. The type of structure is determined by the value of Level.</param>
        /// <param name="dwBuf">The size, in bytes, of the buffer pointed to by pPrinter.</param>
        /// <param name="dwNeeded">A pointer to a variable that the function sets to the size, in bytes, of the printer information. If cbBuf is smaller than this value, GetPrinter fails, and the value represents the required buffer size. If cbBuf is equal to or greater than this value, GetPrinter succeeds, and the value represents the number of bytes stored in the buffer.</param>
        /// <returns></returns>
        [DllImport("winspool.drv", EntryPoint = "GetPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        internal static extern bool GetPrinter(
            IntPtr hPrinter,
            Int32 dwLevel,
            IntPtr pPrinter,
            Int32 dwBuf,
            out Int32 dwNeeded);


        /// <summary>
        /// The OpenPrinter function retrieves a handle to the specified printer or print server or other types of handles in the print subsystem.
        /// MSDN:https://msdn.microsoft.com/en-us/library/windows/desktop/dd162751(v=vs.85).aspx
        /// </summary>
        /// <param name="szPrinter">A pointer to a null-terminated string that specifies the name of the printer or print server, the printer object, the XcvMonitor, or the XcvPort.  For a printer object use: PrinterName, Job xxxx.For an XcvMonitor, use: ServerName, XcvMonitor MonitorName. For an XcvPort, use: ServerName, XcvPort PortName.If NULL, it indicates the local printer server.</param>
        /// <param name="hPrinter">A pointer to a variable that receives a handle (not thread safe) to the open printer or print server object. The phPrinter parameter can return an Xcv handle for use with the XcvData function.For more information about XcvData, see the DDK.</param>
        /// <param name="pDefault">A pointer to a PRINTER_DEFAULTS structure. This value can be NULL.</param>
        /// <returns></returns>
        [DllImport("winspool.drv", EntryPoint = "OpenPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        internal static extern bool OpenPrinterA(
            [MarshalAs(UnmanagedType.LPStr)] string szPrinter,
            out IntPtr hPrinter,
            IntPtr pDefault); //ref PRINTER_DEFAULTS pd);

        [DllImport("winspool.Drv", EntryPoint = "OpenPrinterW", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
        public static extern bool OpenPrinterW(
            [MarshalAs(UnmanagedType.LPWStr)] string szPrinter,
            out IntPtr hPrinter,
            IntPtr pDefault); //ref PRINTER_DEFAULTS pd);

        /// <summary>
        /// The SetPrinter function sets the data for a specified printer or sets the state of the specified printer by pausing printing, resuming printing, or clearing all print jobs.
        /// MSDN:https://msdn.microsoft.com/en-us/library/windows/desktop/dd145082(v=vs.85).aspx
        /// </summary>
        /// <param name="hPrinter"></param>
        /// <param name="Level"></param>
        /// <param name="pPrinter"></param>
        /// <param name="Command"></param>
        /// <returns></returns>
        [DllImport("winspool.drv", CharSet = CharSet.Ansi, SetLastError = true)]
        internal static extern bool SetPrinter(
            IntPtr hPrinter,
            int Level, IntPtr pPrinter,
            int Command);

        /// <summary>
        /// The GetDefaultPrinter function retrieves the printer name of the default printer for the current user on the local computer.
        /// MSDN:https://msdn.microsoft.com/en-us/library/windows/desktop/dd144876(v=vs.85).aspx
        /// </summary>
        /// <param name="pszBuffer">A pointer to a buffer that receives a null-terminated character string containing the default printer name. If this parameter is NULL, the function fails and the variable pointed to by pcchBuffer returns the required buffer size, in characters.</param>
        /// <param name="size"> [in, out] On input, specifies the size, in characters, of the pszBuffer buffer.On output, receives the size, in characters, of the printer name string, including the terminating null character.</param>
        /// <returns></returns>
        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern bool GetDefaultPrinter(StringBuilder pszBuffer, ref int size);


        /// <summary>
        /// The DeviceCapabilities function retrieves the capabilities of a printer driver.
        /// MSDN:https://msdn.microsoft.com/en-us/library/windows/desktop/dd183552(v=vs.85).aspx
        /// </summary>
        /// <param name="device">A pointer to a null-terminated string that contains the name of the printer. Note that this is the name of the printer, not of the printer driver.</param>
        /// <param name="port">A pointer to a null-terminated string that contains the name of the port to which the device is connected, such as LPT1.</param>
        /// <param name="capability">The capabilities to be queried. This parameter can be one of the following values.</param>
        /// <param name="outputBuffer">A pointer to an array. The format of the array depends on the setting of the fwCapability parameter. If pOutput is NULL, DeviceCapabilities returns the number of bytes required for the output data.</param>
        /// <param name="deviceMode">A pointer to a DEVMODE structure. If this parameter is NULL, DeviceCapabilities retrieves the current default initialization values for the specified printer driver. Otherwise, the function retrieves the values contained in the structure to which pDevMode points.</param>
        /// <returns></returns>
        [DllImport("winspool.drv", EntryPoint = "DeviceCapabilitiesA", SetLastError = true)]
        internal static extern Int32 DeviceCapabilities(
                               [MarshalAs(UnmanagedType.LPStr)] String device,
                               [MarshalAs(UnmanagedType.LPStr)] String port,
                               Int16 capability,
                               IntPtr outputBuffer,
                               IntPtr deviceMode);

        /// <summary>
        /// The EnumPrinters function enumerates available printers, print servers, domains, or print providers.
        /// MSDN:https://msdn.microsoft.com/en-us/library/windows/desktop/dd162692(v=vs.85).aspx
        /// </summary>
        /// <param name="flags">he types of print objects that the function should enumerate. This value can be one or more of the following values.</param>
        /// <param name="printerName">name of printer object</param>
        /// <param name="level">The type of data structures pointed to by pPrinterEnum. Valid values are 1, 2, 4, and 5, which correspond to the PRINTER_INFO_1, PRINTER_INFO_2 , PRINTER_INFO_4, and PRINTER_INFO_5 data structures. This value can be 1, 2, 4, or 5.</param>
        /// <param name="buffer"></param>
        /// <param name="bufferSize">The size, in bytes, of the buffer pointed to by pPrinterEnum.</param>
        /// <param name="requiredBufferSize">bytes received or required</param>
        /// <param name="numPrintersReturned">number of printers enumerated</param>
        /// <returns></returns>
        [DllImport("winspool.drv", SetLastError = true)]
        internal static extern bool EnumPrintersW(Int32 flags,
            [MarshalAs(UnmanagedType.LPTStr)] string printerName,
            Int32 level,
            IntPtr buffer,
            Int32 bufferSize,
            out Int32 requiredBufferSize,
            out Int32 numPrintersReturned);

        /// <summary>
        /// The GetPrinterDriver function retrieves driver data for the specified printer. If the driver is not installed on the local computer, GetPrinterDriver installs it.
        /// MSDN:https://msdn.microsoft.com/en-us/library/windows/desktop/dd144914(v=vs.85).aspx
        /// </summary>
        /// <param name="hPrinter"></param>
        /// <param name="pEnvironment"></param>
        /// <param name="Level"></param>
        /// <param name="pDriverInfo"></param>
        /// <param name="cbBuf"></param>
        /// <param name="pcbNeeded"></param>
        /// <returns></returns>
        [DllImport("winspool.drv", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int GetPrinterDriver(IntPtr hPrinter, string pEnvironment, int Level, IntPtr pDriverInfo, int cbBuf, ref int pcbNeeded);


        [DllImport("kernel32.dll", EntryPoint = "GetLastError", SetLastError = false, ExactSpelling = true, CallingConvention = CallingConvention.StdCall), SuppressUnmanagedCodeSecurityAttribute()]
        internal static extern Int32 GetLastError();




        [DllImport("GDI32.dll", EntryPoint = "CreateDC", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = false, CallingConvention = CallingConvention.StdCall),
        SuppressUnmanagedCodeSecurityAttribute()]
        internal static extern IntPtr CreateDC([MarshalAs(UnmanagedType.LPTStr)]
            string pDrive,
            [MarshalAs(UnmanagedType.LPTStr)] string pName,
            [MarshalAs(UnmanagedType.LPTStr)] string pOutput,
            ref DEVMODE pDevMode);

        [DllImport("GDI32.dll", EntryPoint = "ResetDC", SetLastError = true,
             CharSet = CharSet.Unicode, ExactSpelling = false,
             CallingConvention = CallingConvention.StdCall),
        SuppressUnmanagedCodeSecurityAttribute()]
        internal static extern IntPtr ResetDC(
            IntPtr hDC,
            ref DEVMODE
            pDevMode);

        [DllImport("GDI32.dll", EntryPoint = "DeleteDC", SetLastError = true,
             CharSet = CharSet.Unicode, ExactSpelling = false,
             CallingConvention = CallingConvention.StdCall),
        SuppressUnmanagedCodeSecurityAttribute()]
        internal static extern bool DeleteDC(IntPtr hDC);


        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessageTimeout(
            IntPtr windowHandle,
            uint Msg,
            IntPtr wParam,
            IntPtr lParam,
            SendMessageTimeoutFlags flags,
            uint timeout,
            out IntPtr result
            );

        #endregion

        #region "Data structure"

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct DRIVER_INFO_3
        {
            public uint cVersion;
            public string pName;
            public string pEnvironment;
            public string pDriverPath;
            public string pDataFile;
            public string pConfigFile;
            public string pHelpFile;
            public IntPtr pDependentFiles;
            public string pMonitorName;
            public string pDefaultDataType;
        }

        // Structure and API declarions:
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct DOCINFOA
        {
            [MarshalAs(UnmanagedType.LPStr)]
            public string pDocName;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pOutputFile;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pDataType;
        }
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct DOCINFOW
        {
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pDocName;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pOutputFile;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string pDataType;
        }

        /// <summary>
        ///  纸张存取权限等信息
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct PRINTER_DEFAULTS
        {
            public int pDatatype;
            public int pDevMode;
            public int DesiredAccess;//对打印机的读取权限
        }


        //纸张方向
        public enum PageOrientation
        {
            DMORIENT_PORTRAIT = 1,//直向
            DMORIENT_LANDSCAPE = 2,//橫向
        }

        /// <summary>
        /// 纸张类型
        /// </summary>
        public enum PaperSize
        {
            DMPAPER_LETTER = 1, // Letter 8 1/2 x 11 in
            DMPAPER_LETTERSMALL = 2, // Letter Small 8 1/2 x 11 in
            DMPAPER_TABLOID = 3, // Tabloid 11 x 17 in
            DMPAPER_LEDGER = 4, // Ledger 17 x 11 in
            DMPAPER_LEGAL = 5, // Legal 8 1/2 x 14 in
            DMPAPER_STATEMENT = 6, // Statement 5 1/2 x 8 1/2 in
            DMPAPER_EXECUTIVE = 7, // Executive 7 1/4 x 10 1/2 in
            DMPAPER_A3 = 8, // A3 297 x 420 mm
            DMPAPER_A4 = 9, // A4 210 x 297 mm
            DMPAPER_A4SMALL = 10, // A4 Small 210 x 297 mm
            DMPAPER_A5 = 11, // A5 148 x 210 mm
            DMPAPER_B4 = 12, // B4 250 x 354
            DMPAPER_B5 = 13, // B5 182 x 257 mm
            DMPAPER_FOLIO = 14, // Folio 8 1/2 x 13 in
            DMPAPER_QUARTO = 15, // Quarto 215 x 275 mm
            DMPAPER_10X14 = 16, // 10x14 in
            DMPAPER_11X17 = 17, // 11x17 in
            DMPAPER_NOTE = 18, // Note 8 1/2 x 11 in
            DMPAPER_ENV_9 = 19, // Envelope #9 3 7/8 x 8 7/8
            DMPAPER_ENV_10 = 20, // Envelope #10 4 1/8 x 9 1/2
            DMPAPER_ENV_11 = 21, // Envelope #11 4 1/2 x 10 3/8
            DMPAPER_ENV_12 = 22, // Envelope #12 4 /276 x 11
            DMPAPER_ENV_14 = 23, // Envelope #14 5 x 11 1/2
            DMPAPER_CSHEET = 24, // C size sheet
            DMPAPER_DSHEET = 25, // D size sheet
            DMPAPER_ESHEET = 26, // E size sheet
            DMPAPER_ENV_DL = 27, // Envelope DL 110 x 220mm
            DMPAPER_ENV_C5 = 28, // Envelope C5 162 x 229 mm
            DMPAPER_ENV_C3 = 29, // Envelope C3 324 x 458 mm
            DMPAPER_ENV_C4 = 30, // Envelope C4 229 x 324 mm
            DMPAPER_ENV_C6 = 31, // Envelope C6 114 x 162 mm
            DMPAPER_ENV_C65 = 32, // Envelope C65 114 x 229 mm
            DMPAPER_ENV_B4 = 33, // Envelope B4 250 x 353 mm
            DMPAPER_ENV_B5 = 34, // Envelope B5 176 x 250 mm
            DMPAPER_ENV_B6 = 35, // Envelope B6 176 x 125 mm
            DMPAPER_ENV_ITALY = 36, // Envelope 110 x 230 mm
            DMPAPER_ENV_MONARCH = 37, // Envelope Monarch 3.875 x 7.5 in
            DMPAPER_ENV_PERSONAL = 38, // 6 3/4 Envelope 3 5/8 x 6 1/2 in
            DMPAPER_FANFOLD_US = 39, // US Std Fanfold 14 7/8 x 11 in
            DMPAPER_FANFOLD_STD_GERMAN = 40, // German Std Fanfold 8 1/2 x 12 in
            DMPAPER_FANFOLD_LGL_GERMAN = 41, // German Legal Fanfold 8 1/2 x 13 in
            DMPAPER_USER = 256,// user defined
            DMPAPER_FIRST = DMPAPER_LETTER,
            DMPAPER_LAST = DMPAPER_USER,
        }


        /// <summary>
        /// 纸张来源
        /// </summary>
        public enum PaperSource
        {
            DMBIN_UPPER = 1,
            DMBIN_LOWER = 2,
            DMBIN_MIDDLE = 3,
            DMBIN_MANUAL = 4,
            DMBIN_ENVELOPE = 5,
            DMBIN_ENVMANUAL = 6,
            DMBIN_AUTO = 7,
            DMBIN_TRACTOR = 8,
            DMBIN_SMALLFMT = 9,
            DMBIN_LARGEFMT = 10,
            DMBIN_LARGECAPACITY = 11,
            DMBIN_CASSETTE = 14,
            DMBIN_FORMSOURCE = 15,
            DMRES_DRAFT = -1,
            DMRES_LOW = -2,
            DMRES_MEDIUM = -3,
            DMRES_HIGH = -4
        }


        /// <summary>
        /// 是否要双面打印
        /// </summary>
        public enum PageDuplex
        {
            DMDUP_HORIZONTAL = 3,
            DMDUP_SIMPLEX = 1,
            DMDUP_VERTICAL = 2
        }


        /// <summary>
        /// 需要变更的打印参数
        /// </summary>
        public struct PrinterSettingsInfo
        {
            public PageOrientation Orientation; //列印方向
            public PaperSize Size;              //列印紙張類型（用數字表示 256為用戶自定紙張）
            public PaperSource source;          //紙張來源
            public PageDuplex Duplex;           //是否雙面列印等信息
            public int pLength;                 //紙張的高
            public int pWidth;                  //紙張的寬
            public int pmFields;                //需改變的信息進行"|"運算後的和
            public string pFormName;            //紙張的名字
        }

        //PRINTER_INFO_2 - 打印机信息結構包含 1..9 個等級，詳細信息請參考API
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct PRINTER_INFO_2
        {
            [MarshalAs(UnmanagedType.LPStr)]
            public string pServerName;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pPrinterName;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pShareName;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pPortName;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pDriverName;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pComment;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pLocation;
            public IntPtr pDevMode;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pSepFile;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pPrintProcessor;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pDatatype;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pParameters;
            public IntPtr pSecurityDescriptor;
            public Int32 Attributes;
            public Int32 Priority;
            public Int32 DefaultPriority;
            public Int32 StartTime;
            public Int32 UntilTime;
            public Int32 Status;
            public Int32 cJobs;
            public Int32 AveragePPM;
        }


        //PRINTER_INFO_5 - 打印机信息結構包含 1..9 個等級，詳細信息請參考API
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct PRINTER_INFO_5
        {
            [MarshalAs(UnmanagedType.LPTStr)]
            public String PrinterName;
            [MarshalAs(UnmanagedType.LPTStr)]
            public String PortName;
            [MarshalAs(UnmanagedType.U4)]
            public Int32 Attributes;
            [MarshalAs(UnmanagedType.U4)]
            public Int32 DeviceNotSelectedTimeout;
            [MarshalAs(UnmanagedType.U4)]
            public Int32 TransmissionRetryTimeout;
        }


        //PRINTER_INFO_9
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        internal struct PRINTER_INFO_9
        {
            public IntPtr pDevMode;
        }

        /// <summary>
        /// The DEVMODE data structure contains information about the initialization and environment of a printer or a display device
        ///DEVMODE結構包含了印表機（或顯示設置)的初始化和當前狀態信息,詳細信息請參考API
        /// </summary>
        private const short CCDEVICENAME = 32;
        private const short CCFORMNAME = 32;
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct DEVMODE
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCDEVICENAME)]
            public string dmDeviceName;
            public short dmSpecVersion;
            public short dmDriverVersion;
            public short dmSize;
            public short dmDriverExtra;
            public int dmFields;
            public short dmOrientation;
            public short dmPaperSize;
            public short dmPaperLength;
            public short dmPaperWidth;
            public short dmScale;
            public short dmCopies;
            public short dmDefaultSource;
            public short dmPrintQuality;
            public short dmColor;
            public short dmDuplex;
            public short dmYResolution;
            public short dmTTOption;
            public short dmCollate;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCFORMNAME)]
            public string dmFormName;
            public short dmUnusedPadding;
            public short dmBitsPerPel;
            public int dmPelsWidth;
            public int dmPelsHeight;
            public int dmDisplayFlags;
            public int dmDisplayFrequency;
        }

        //SendMessageTimeout Flags
        [Flags]
        public enum SendMessageTimeoutFlags : uint
        {
            SMTO_NORMAL = 0x0000,
            SMTO_BLOCK = 0x0001,
            SMTO_ABORTIFHUNG = 0x0002,
            SMTO_NOTIMEOUTIFNOTHUNG = 0x0008
        }
        #endregion

        #region "const Variables"

        //DEVMODE.dmFields
        const int DM_FORMNAME = 0x10000;//改變紙張名稱時需在dmFields設置此常數
        const int DM_PAPERSIZE = 0x0002;//改變紙張類型時需在dmFields設置此常數
        const int DM_PAPERLENGTH = 0x0004;//改變紙張長度時需在dmFields設置此常數
        const int DM_PAPERWIDTH = 0x0008;//改變紙張寬度時需在dmFields設置此常數
        const int DM_DUPLEX = 0x1000;//改變紙張是否雙面列印時需在dmFields設置此常數
        const int DM_ORIENTATION = 0x0001;//改變紙張方向時需在dmFields設置此常數

        //用於改變DocumentProperties的參數，詳細信息請參考API
        const int DM_IN_BUFFER = 8;
        const int DM_OUT_BUFFER = 2;

        //用於設置對印表機的存取權限
        const int PRINTER_ACCESS_ADMINISTER = 0x4;
        const int PRINTER_ACCESS_USE = 0x8;
        const int STANDARD_RIGHTS_REQUIRED = 0xF0000;
        const int PRINTER_ALL_ACCESS = (STANDARD_RIGHTS_REQUIRED | PRINTER_ACCESS_ADMINISTER | PRINTER_ACCESS_USE);

        //得到指定列印的所有紙張
        const int PRINTER_ENUM_LOCAL = 2;
        const int PRINTER_ENUM_CONNECTIONS = 4;
        const int DC_PAPERNAMES = 16;
        const int DC_PAPERS = 2;
        const int DC_PAPERSIZE = 3;

        //sendMessageTimeOut
        const int WM_SETTINGCHANGE = 0x001A;
        const int HWND_BROADCAST = 0xffff;
        #endregion

        #region printer method

        private static IEnumerable<string> ReadMultiSz(IntPtr ptr)
        {
            if (ptr == IntPtr.Zero)
            {
                yield break;
            }

            var builder = new StringBuilder();
            var pos = ptr;

            while (true)
            {
                var c = (char)Marshal.ReadInt16(pos);

                if (c == '\0')
                {
                    if (builder.Length == 0)
                    {
                        break;
                    }

                    yield return builder.ToString();
                    builder = new StringBuilder();
                }
                else
                {
                    builder.Append(c);
                }

                pos += 2;
            }
        }

        /// <summary>
        /// 获取指定的打印机的驱动依赖文件
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetPrinterDriverDependentFiles(IntPtr handle)
        {
            int bufferSize = 0;

            if (GetPrinterDriver(handle, null, 3, IntPtr.Zero, 0, ref bufferSize) != 0 || Marshal.GetLastWin32Error() != 122) // 122 = ERROR_INSUFFICIENT_BUFFER
            {
                throw new Win32Exception();
            }

            var ptr = Marshal.AllocHGlobal(bufferSize);

            try
            {
                if (GetPrinterDriver(handle, null, 3, ptr, bufferSize, ref bufferSize) == 0)
                {
                    throw new Win32Exception();
                }

                var di3 = (DRIVER_INFO_3)Marshal.PtrToStructure(ptr, typeof(DRIVER_INFO_3));

                return ReadMultiSz(di3.pDependentFiles).ToList(); // We need a list because FreeHGlobal will be called on return
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
        }

        /// <summary>
        /// 是否为XPS打印机
        /// </summary>
        /// <param name="printerHandler"></param>
        /// <returns></returns>
        private static bool IsXPSDriver(IntPtr printerHandler)
        {
            var files = GetPrinterDriverDependentFiles(printerHandler);

            return files.Any(f => f.EndsWith("pipelineconfig.xml", StringComparison.InvariantCultureIgnoreCase));
        }


        // SendBytesToPrinter()  
        // When the function is given a printer name and an unmanaged array  
        // of bytes, the function sends those bytes to the print queue.  
        // Returns true on success, false on failure.  
        /// <summary>
        /// 发送字节到打印机设备
        /// 注意：直接发送字节数据到指定的打印机 是不对的。输出的字节 必须是特定格式的指令字节，也就是在发送前 需要转换文档
        /// Send PostScript, PCL or other print file types directly to a printer.
        /// XPS打印  与GDI 设备打印 是两个不同的分支结构
        /// 在MSDN:https://msdn.microsoft.com/en-us/library/windows/desktop/ff686814(v=vs.85).aspx 中解释了这个流程
        /// 所以 XPS 打印 需要使用XPS API---修改文档类型为 XPS_PASS
        /// 
        /// </summary>
        /// <param name="szPrinterName"></param>
        /// <param name="docName"></param>
        /// <param name="pBytes"></param>
        /// <param name="dwCount"></param>
        /// <returns></returns>
        public static bool SendBytesToPrinter(string szPrinterName, string docName, IntPtr pBytes, Int32 dwCount)
        {
            Int32 dwError = 0, dwWritten = 0;
            IntPtr hPrinter = new IntPtr(0);
            DOCINFOW di = new DOCINFOW();
            bool bSuccess = false; // Assume failure unless you specifically succeed.  

            //文档名称
            di.pDocName = docName; //"My C#.NET RAW Document";


            // Open the printer.  
            if (OpenPrinterW(szPrinterName.Normalize(), out hPrinter, IntPtr.Zero))
            {
                di.pDataType = IsXPSDriver(hPrinter) ? "XPS_PASS" : "RAW";//"RAW";------是否是XPS打印

                // Start a document.  
                if (StartDocPrinterW(hPrinter, 1, di))
                {
                    // Start a page.  
                    if (StartPagePrinter(hPrinter))
                    {
                        // Write your bytes.  
                        bSuccess = WritePrinter(hPrinter, pBytes, dwCount, out dwWritten);
                        EndPagePrinter(hPrinter);
                    }
                    EndDocPrinter(hPrinter);
                }
                ClosePrinter(hPrinter);
            }
            // If you did not succeed, GetLastError may give more information  
            // about why not.  
            if (bSuccess == false)
            {
                dwError = Marshal.GetLastWin32Error();
            }
            return bSuccess;
        }

        public static bool SendFileToPrinter(string szPrinterName, string szFileName, string docName)
        {
            FileStream fs = null;
            BinaryReader br = null;
            try
            {


                // Open the file.  
                fs = new FileStream(szFileName, FileMode.Open);
                // Create a BinaryReader on the file.  
                br = new BinaryReader(fs);
                // Dim an array of bytes big enough to hold the file's contents.  
                Byte[] bytes = new Byte[fs.Length];
                bool bSuccess = false;
                // Your unmanaged pointer.  
                IntPtr pUnmanagedBytes = new IntPtr(0);
                int nLength;

                nLength = Convert.ToInt32(fs.Length);
                // Read the contents of the file into the array.  
                bytes = br.ReadBytes(nLength);
                // Allocate some unmanaged memory for those bytes.  
                pUnmanagedBytes = Marshal.AllocCoTaskMem(nLength);
                // Copy the managed byte array into the unmanaged array.  
                Marshal.Copy(bytes, 0, pUnmanagedBytes, nLength);
                // Send the unmanaged bytes to the printer.  
                bSuccess = SendBytesToPrinter(szPrinterName, docName, pUnmanagedBytes, nLength);
                // Free the unmanaged memory that you allocated earlier.  
                Marshal.FreeCoTaskMem(pUnmanagedBytes);
                return bSuccess;
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                if (null != br)
                {
                    br.Dispose();
                }
                if (null != fs)
                {
                    fs.Dispose();
                }

            }
        }

        public static bool SendStringToPrinter(string szPrinterName, string szString, string docName)
        {
            IntPtr pBytes;
            Int32 dwCount;
            // How many characters are in the string?  
            dwCount = szString.Length;
            // Assume that the printer is expecting ANSI text, and then convert  
            // the string to ANSI text.  
            pBytes = Marshal.StringToCoTaskMemAnsi(szString);
            // Send the converted ANSI string to the printer.  
            SendBytesToPrinter(szPrinterName, docName, pBytes, dwCount);
            Marshal.FreeCoTaskMem(pBytes);
            return true;
        }


        public static bool OpenPrinterEx(string szPrinter, out IntPtr hPrinter, ref PRINTER_DEFAULTS pd)
        {
            bool bRet = OpenPrinterW(szPrinter, out hPrinter, IntPtr.Zero);
            return bRet;
        }

        /// <summary>
        /// 获取指定的打印机的信息
        /// </summary>
        /// <param name="PrinterName">打印机名称</param>
        /// <returns></returns>
        public static DEVMODE GetPrinterDevMode(string PrinterName)
        {
            if (PrinterName == string.Empty || PrinterName == null)
            {
                PrinterName = GetDefaultPrinterName();
            }

            PRINTER_DEFAULTS pd = new PRINTER_DEFAULTS();
            pd.pDatatype = 0;
            pd.pDevMode = 0;
            pd.DesiredAccess = PRINTER_ALL_ACCESS;
            // Michael: some printers (e.g. network printer) do not allow PRINTER_ALL_ACCESS and will cause Access Is Denied error.
            // When this happen, try PRINTER_ACCESS_USE.

            IntPtr hPrinter = new System.IntPtr();
            if (!OpenPrinterEx(PrinterName, out hPrinter, ref pd))
            {
                lastError = Marshal.GetLastWin32Error();
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            int nBytesNeeded = 0;
            GetPrinter(hPrinter, 2, IntPtr.Zero, 0, out nBytesNeeded);
            if (nBytesNeeded <= 0)
            {
                throw new System.Exception("Unable to allocate memory");
            }


            DEVMODE dm;

            // Allocate enough space for PRINTER_INFO_2... {ptrPrinterIn fo = Marshal.AllocCoTaskMem(nBytesNeeded)};
            IntPtr ptrPrinterInfo = Marshal.AllocHGlobal(nBytesNeeded);

            // The second GetPrinter fills in all the current settings, so all you 
            // need to do is modify what you're interested in...
            nRet = Convert.ToInt32(GetPrinter(hPrinter, 2, ptrPrinterInfo, nBytesNeeded, out nJunk));
            if (nRet == 0)
            {
                lastError = Marshal.GetLastWin32Error();
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            PRINTER_INFO_2 pinfo = new PRINTER_INFO_2();
            pinfo = (PRINTER_INFO_2)Marshal.PtrToStructure(ptrPrinterInfo, typeof(PRINTER_INFO_2));
            IntPtr Temp = new IntPtr();
            if (pinfo.pDevMode == IntPtr.Zero)
            {
                // If GetPrinter didn't fill in the DEVMODE, try to get it by calling
                // DocumentProperties...
                IntPtr ptrZero = IntPtr.Zero;
                //get the size of the devmode structure
                int sizeOfDevMode = DocumentProperties(IntPtr.Zero, hPrinter, PrinterName, IntPtr.Zero, IntPtr.Zero, 0);

                IntPtr ptrDM = Marshal.AllocCoTaskMem(sizeOfDevMode);
                int i;
                i = DocumentProperties(IntPtr.Zero, hPrinter, PrinterName, ptrDM, ptrZero, DM_OUT_BUFFER);
                if ((i < 0) || (ptrDM == IntPtr.Zero))
                {
                    //Cannot get the DEVMODE structure.
                    throw new System.Exception("Cannot get DEVMODE data");
                }
                pinfo.pDevMode = ptrDM;
            }
            intError = DocumentProperties(IntPtr.Zero, hPrinter, PrinterName, IntPtr.Zero, Temp, 0);

            //IntPtr yDevModeData = Marshal.AllocCoTaskMem(i1);
            IntPtr yDevModeData = Marshal.AllocHGlobal(intError);
            intError = DocumentProperties(IntPtr.Zero, hPrinter, PrinterName, yDevModeData, Temp, 2);
            dm = (DEVMODE)Marshal.PtrToStructure(yDevModeData, typeof(DEVMODE));//從記憶空間中取出印表機設備信息
            //nRet = DocumentProperties(IntPtr.Zero, hPrinter, sPrinterName, yDevModeData
            // , ref yDevModeData, (DM_IN_BUFFER | DM_OUT_BUFFER));
            if ((nRet == 0) || (hPrinter == IntPtr.Zero))
            {
                lastError = Marshal.GetLastWin32Error();
                //string myErrMsg = GetErrorMessage(lastError);
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            ClosePrinter(hPrinter);

            return dm;
        }

        /// <summary>
        /// 获取默认纸张的示范
        /// </summary>
        public static void GetDefaultPaperSize()
        {
            var devMode = GetPrinterDevMode(null);
            string s = String.Format("{0} : {1} x {2}", devMode.dmPaperSize, devMode.dmPaperWidth, devMode.dmPaperLength);
            Console.WriteLine(s);
        }

        /// <summary>
        /// 设置纸张大小
        /// </summary>
        /// <param name="printerName"></param>
        /// <param name="formName"></param>
        /// <param name="paperWidth"></param>
        /// <param name="paperHeight"></param>
        public static void SetDefaultPaperSize(string printerName, string formName, int paperWidth, int paperHeight)
        {
            //string formName = "User defined";
            //int paperWidth = 1016;
            //int paperHeight = 2032;

            if (!IsPaperSize(formName, paperWidth, paperWidth))
            {
                PrinterSettingsInfo pd = new PrinterSettingsInfo();
                pd.Duplex = 0;              // 不設定是否雙面列印（不設定即採預設值，以下類推）
                pd.Orientation = 0;         // 不設定列印方向
                pd.pFormName = formName;    // 纸张名字
                pd.pLength = paperWidth;        // 設定打印机的高度
                pd.pWidth = paperWidth;          // 設定打印机的宽度                
                                                 //pd.Size = PaperSize.DMPAPER_USER; // 自定义纸张
                ModifyPrinterSettings(printerName, ref pd);
            }
        }

        /// <summary>
        /// 判断当前打印机设定的纸张是否是传入的大小
        /// </summary>
        /// <param name="FormName">紙張名稱。</param>
        /// <param name="width">寬。Unit: 1/10 of a millimeter.</param>
        /// <param name="length">高。Unit: 1/10 of a millimeter.</param>
        /// <returns>如果預設印表機的 DEVMODE 結構中的紙張大小與指定之 width 和 height 相同則傳回 true，否則傳回 false。</returns>
        public static bool IsPaperSize(string FormName, int width, int length)
        {
            DEVMODE dm = PrinterHelper.GetPrinterDevMode(null);
            if (FormName == dm.dmFormName && width == dm.dmPaperWidth && length == dm.dmPaperLength)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 改变打印机的设置
        /// </summary>
        /// <param name="printerName">印表機的名字,如果為空，自動得到預設印表機的名字</param>
        /// <param name="prnSettings">需改變信息</param>
        /// <returns>是否改變成功</returns>
        public static void ModifyPrinterSettings(string printerName, ref PrinterSettingsInfo prnSettings)
        {
            PRINTER_INFO_9 printerInfo;
            printerInfo.pDevMode = IntPtr.Zero;
            if (String.IsNullOrEmpty(printerName))
            {
                printerName = GetDefaultPrinterName();
            }

            IntPtr hPrinter = new System.IntPtr();

            PRINTER_DEFAULTS prnDefaults = new PRINTER_DEFAULTS();
            prnDefaults.pDatatype = 0;
            prnDefaults.pDevMode = 0;
            prnDefaults.DesiredAccess = PRINTER_ALL_ACCESS;

            if (!OpenPrinterEx(printerName, out hPrinter, ref prnDefaults))
            {
                return;
            }

            IntPtr ptrPrinterInfo = IntPtr.Zero;
            try
            {
                //得到結構體DEVMODE的大小
                int iDevModeSize = DocumentProperties(IntPtr.Zero, hPrinter, printerName, IntPtr.Zero, IntPtr.Zero, 0);
                if (iDevModeSize < 0)
                    throw new ApplicationException("Cannot get the size of the DEVMODE structure.");

                //分配指向結構體DEVMODE的記憶空間緩沖區
                IntPtr hDevMode = Marshal.AllocCoTaskMem(iDevModeSize + 100);

                //得到一個指向 DEVMODE 結構的指標
                nRet = DocumentProperties(IntPtr.Zero, hPrinter, printerName, hDevMode, IntPtr.Zero, DM_OUT_BUFFER);
                if (nRet < 0)
                    throw new ApplicationException("Cannot get the size of the DEVMODE structure.");
                //給dm賦值
                DEVMODE dm = (DEVMODE)Marshal.PtrToStructure(hDevMode, typeof(DEVMODE));

                if ((((int)prnSettings.Duplex < 0) || ((int)prnSettings.Duplex > 3)))
                {
                    throw new ArgumentOutOfRangeException("nDuplexSetting", "nDuplexSetting is incorrect.");
                }
                else
                {
                    // 更改印表機設定
                    if ((int)prnSettings.Size != 0) //是否改變紙張類型
                    {
                        dm.dmPaperSize = (short)prnSettings.Size;
                        dm.dmFields |= DM_PAPERSIZE;
                    }
                    if (prnSettings.pWidth != 0)    //是否改變紙張寬度
                    {
                        dm.dmPaperWidth = (short)prnSettings.pWidth;
                        dm.dmFields |= DM_PAPERWIDTH;
                    }
                    if (prnSettings.pLength != 0)   //是否改變紙張高度
                    {
                        dm.dmPaperLength = (short)prnSettings.pLength;
                        dm.dmFields |= DM_PAPERLENGTH;
                    }
                    if (!String.IsNullOrEmpty(prnSettings.pFormName))    //是否改變紙張名稱
                    {
                        dm.dmFormName = prnSettings.pFormName;
                        dm.dmFields |= DM_FORMNAME;
                    }
                    if ((int)prnSettings.Orientation != 0)  //是否改變紙張方向
                    {
                        dm.dmOrientation = (short)prnSettings.Orientation;
                        dm.dmFields |= DM_ORIENTATION;
                    }
                    Marshal.StructureToPtr(dm, hDevMode, true);

                    //得到 printer info 的大小
                    nRet = DocumentProperties(IntPtr.Zero, hPrinter, printerName, printerInfo.pDevMode, printerInfo.pDevMode, DM_IN_BUFFER | DM_OUT_BUFFER);
                    if (nRet < 0)
                    {
                        throw new ApplicationException("Unable to set the PrintSetting for this printer");
                    }
                    int nBytesNeeded = 0;
                    GetPrinter(hPrinter, 9, IntPtr.Zero, 0, out nBytesNeeded);
                    if (nBytesNeeded == 0)
                        throw new ApplicationException("GetPrinter failed.Couldn't get the nBytesNeeded for shared PRINTER_INFO_9 structure");

                    //配置記憶體區塊
                    ptrPrinterInfo = Marshal.AllocCoTaskMem(nBytesNeeded);
                    bool bSuccess = GetPrinter(hPrinter, 9, ptrPrinterInfo, nBytesNeeded, out nJunk);
                    if (!bSuccess)
                        throw new ApplicationException("GetPrinter failed.Couldn't get the nBytesNeeded for shared PRINTER_INFO_9 structure");
                    //賦值給printerInfo
                    printerInfo = (PRINTER_INFO_9)Marshal.PtrToStructure(ptrPrinterInfo, printerInfo.GetType());
                    printerInfo.pDevMode = hDevMode;

                    //獲取一個指向 PRINTER_INFO_9 結構的指標
                    Marshal.StructureToPtr(printerInfo, ptrPrinterInfo, true);

                    //設置印表機
                    bSuccess = SetPrinter(hPrinter, 9, ptrPrinterInfo, 0);
                    if (!bSuccess)
                        throw new Win32Exception(Marshal.GetLastWin32Error(), "SetPrinter() failed.Couldn't set the printer settings");

                    // 通知其它 app，印表機設定已經更改 -- Do NOT use because it causes app halt serveral seconds!!
                    /*
                    PrinterHelper.SendMessageTimeout(
                        new IntPtr(HWND_BROADCAST), WM_SETTINGCHANGE, IntPtr.Zero, IntPtr.Zero,
                        PrinterHelper.SendMessageTimeoutFlags.SMTO_NORMAL, 1000, out hDummy);
                     */
                }
            }
            finally
            {
                ClosePrinter(hPrinter);

                //釋放
                if (ptrPrinterInfo == IntPtr.Zero)
                    Marshal.FreeHGlobal(ptrPrinterInfo);
                if (hPrinter == IntPtr.Zero)
                    Marshal.FreeHGlobal(hPrinter);
            }
        }


        /// <summary>
        /// 改變印表機設定的另一個版本。測試過程中曾出現應用程式異常終止而且無任何錯誤訊息。請使用 ModifyPrinterSettings。
        /// </summary>
        /// <param name="printerName">印表機名稱。傳入 null 或空字串表示使用預設印表機。</param>
        /// <param name="PS">需改變信息</param>
        /// <returns>是否改變成功</returns>
        public static bool ModifyPrinterSettings_V2(string printerName, ref PrinterSettingsInfo PS)
        {
            PRINTER_DEFAULTS pd = new PRINTER_DEFAULTS();
            pd.pDatatype = 0;
            pd.pDevMode = 0;
            pd.DesiredAccess = PRINTER_ALL_ACCESS;
            if (String.IsNullOrEmpty(printerName))
            {
                printerName = GetDefaultPrinterName();
            }

            IntPtr hPrinter = new System.IntPtr();

            if (!OpenPrinterEx(printerName, out hPrinter, ref pd))
            {
                lastError = Marshal.GetLastWin32Error();
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
            //呼叫GetPrinter來獲取PRINTER_INFO_2在記憶空間的 bytes 數
            int nBytesNeeded = 0;
            GetPrinter(hPrinter, 2, IntPtr.Zero, 0, out nBytesNeeded);
            if (nBytesNeeded <= 0)
            {
                ClosePrinter(hPrinter);
                return false;
            }
            //為PRINTER_INFO_2分配足夠的記憶空間
            IntPtr ptrPrinterInfo = Marshal.AllocHGlobal(nBytesNeeded);
            if (ptrPrinterInfo == IntPtr.Zero)
            {
                ClosePrinter(hPrinter);
                return false;
            }

            //呼叫GetPrinter填充所的當前設定，也就是你所想改變的信息（ptrPrinterInfo中）
            if (!GetPrinter(hPrinter, 2, ptrPrinterInfo, nBytesNeeded, out nBytesNeeded))
            {
                Marshal.FreeHGlobal(ptrPrinterInfo);
                ClosePrinter(hPrinter);
                return false;
            }
            //把記憶區塊中指向 PRINTER_INFO_2 的指標轉化為 PRINTER_INFO_2 結構
            //如果 GetPrinter 沒有得到 DEVMODE 結構，將嘗試透過 DocumentProperties 來取得 DEVMODE 結構
            PRINTER_INFO_2 pinfo = new PRINTER_INFO_2();
            pinfo = (PRINTER_INFO_2)Marshal.PtrToStructure(ptrPrinterInfo, typeof(PRINTER_INFO_2));
            IntPtr Temp = new IntPtr();
            if (pinfo.pDevMode == IntPtr.Zero)
            {
                // If GetPrinter didn't fill in the DEVMODE, try to get it by calling
                // DocumentProperties...
                IntPtr ptrZero = IntPtr.Zero;
                //get the size of the devmode structure
                nBytesNeeded = DocumentProperties(IntPtr.Zero, hPrinter, printerName, IntPtr.Zero, IntPtr.Zero, 0);
                if (nBytesNeeded <= 0)
                {
                    Marshal.FreeHGlobal(ptrPrinterInfo);
                    ClosePrinter(hPrinter);
                    return false;
                }
                IntPtr ptrDM = Marshal.AllocCoTaskMem(nBytesNeeded);
                int i;
                i = DocumentProperties(IntPtr.Zero, hPrinter, printerName, ptrDM, ptrZero, DM_OUT_BUFFER);
                if ((i < 0) || (ptrDM == IntPtr.Zero))
                {
                    //Cannot get the DEVMODE structure.
                    Marshal.FreeHGlobal(ptrDM);
                    ClosePrinter(ptrPrinterInfo);
                    return false;
                }
                pinfo.pDevMode = ptrDM;
            }
            DEVMODE dm = (DEVMODE)Marshal.PtrToStructure(pinfo.pDevMode, typeof(DEVMODE));

            //修改印表機的設定信息        
            if ((((int)PS.Duplex < 0) || ((int)PS.Duplex > 3)))
            {
                throw new ArgumentOutOfRangeException("nDuplexSetting", "nDuplexSetting is incorrect.");
            }
            else
            {
                if (String.IsNullOrEmpty(printerName))
                {
                    printerName = GetDefaultPrinterName();
                }
                if ((int)PS.Size != 0)//是否改變紙張類型
                {
                    dm.dmPaperSize = (short)PS.Size;
                    dm.dmFields |= DM_PAPERSIZE;
                }
                if (PS.pWidth != 0)//是否改變紙張寬度
                {
                    dm.dmPaperWidth = (short)PS.pWidth;
                    dm.dmFields |= DM_PAPERWIDTH;
                }
                if (PS.pLength != 0)//是否改變紙張高度
                {
                    dm.dmPaperLength = (short)PS.pLength;
                    dm.dmFields |= DM_PAPERLENGTH;
                }
                if (!String.IsNullOrEmpty(PS.pFormName))    //是否改變紙張名稱
                {
                    dm.dmFormName = PS.pFormName;
                    dm.dmFields |= DM_FORMNAME;
                }
                if ((int)PS.Orientation != 0)//是否改變紙張方向
                {
                    dm.dmOrientation = (short)PS.Orientation;
                    dm.dmFields |= DM_ORIENTATION;
                }
                Marshal.StructureToPtr(dm, pinfo.pDevMode, true);
                Marshal.StructureToPtr(pinfo, ptrPrinterInfo, true);
                pinfo.pSecurityDescriptor = IntPtr.Zero;
                //Make sure the driver_Dependent part of devmode is updated...
                nRet = DocumentProperties(IntPtr.Zero, hPrinter, printerName, pinfo.pDevMode, pinfo.pDevMode, DM_IN_BUFFER | DM_OUT_BUFFER);
                if (nRet <= 0)
                {
                    Marshal.FreeHGlobal(ptrPrinterInfo);
                    ClosePrinter(hPrinter);
                    return false;
                }

                //SetPrinter 更新印表機信息
                if (!SetPrinter(hPrinter, 2, ptrPrinterInfo, 0))
                {
                    Marshal.FreeHGlobal(ptrPrinterInfo);
                    ClosePrinter(hPrinter);
                    return false;
                }
                //通知其它應用程序，印表機信息已經更改
                IntPtr hDummy = IntPtr.Zero;
                PrinterHelper.SendMessageTimeout(
                    new IntPtr(HWND_BROADCAST), WM_SETTINGCHANGE, IntPtr.Zero, IntPtr.Zero,
                    PrinterHelper.SendMessageTimeoutFlags.SMTO_NORMAL, 1000, out hDummy);

                //釋放
                if (ptrPrinterInfo == IntPtr.Zero)
                    Marshal.FreeHGlobal(ptrPrinterInfo);
                if (hPrinter == IntPtr.Zero)
                    Marshal.FreeHGlobal(hPrinter);

                return true;

            }
        }


        /// <summary>
        /// 获取默认打印机名称
        /// </summary>
        /// <returns>返回預設印表機的名字</returns>
        public static string GetDefaultPrinterName()
        {
            StringBuilder dp = new StringBuilder(256);
            int size = dp.Capacity;
            if (GetDefaultPrinter(dp, ref size))
            {
                return dp.ToString();
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 得到纸张的kind，如果为 0 表示错误。
        /// </summary>
        /// <param name="printerName">打印机名称。傳入 null 或空字串表示使用預設印表機。</param>
        /// <param name="paperName">紙張名稱，一定要填</param>
        /// <returns>kind</returns>
        public static short GetOnePaper(string printerName, string paperName)
        {

            short kind = 0;
            if (String.IsNullOrEmpty(printerName))
                printerName = GetDefaultPrinterName();
            PRINTER_INFO_5 info5;
            int requiredSize;
            int numPrinters;
            bool foundPrinter = EnumPrintersW(PRINTER_ENUM_LOCAL | PRINTER_ENUM_CONNECTIONS,
                string.Empty, 5, IntPtr.Zero, 0, out requiredSize, out numPrinters);

            int info5Size = requiredSize;
            IntPtr info5Ptr = Marshal.AllocHGlobal(info5Size);
            IntPtr buffer = IntPtr.Zero;
            try
            {
                foundPrinter = EnumPrintersW(PRINTER_ENUM_LOCAL | PRINTER_ENUM_CONNECTIONS,
                    string.Empty, 5, info5Ptr, info5Size, out requiredSize, out numPrinters);

                string port = null;
                for (int i = 0; i < numPrinters; i++)
                {
                    info5 = (PRINTER_INFO_5)Marshal.PtrToStructure(
                        (IntPtr)((i * Marshal.SizeOf(typeof(PRINTER_INFO_5))) + (int)info5Ptr),
                        typeof(PRINTER_INFO_5));
                    if (info5.PrinterName == printerName)
                    {
                        port = info5.PortName;
                    }
                }

                int numNames = DeviceCapabilities(printerName, port, DC_PAPERNAMES, IntPtr.Zero, IntPtr.Zero);
                if (numNames < 0)
                {
                    int errorCode = GetLastError();
                    Console.WriteLine("Number of names = {1}: {0}", errorCode, numNames);
                    return 0;
                }

                buffer = Marshal.AllocHGlobal(numNames * 64);
                numNames = DeviceCapabilities(printerName, port, DC_PAPERNAMES, buffer, IntPtr.Zero);
                if (numNames < 0)
                {
                    int errorCode = GetLastError();
                    Console.WriteLine("Number of names = {1}: {0}", errorCode, numNames);
                    return 0;
                }
                string[] names = new string[numNames];
                for (int i = 0; i < numNames; i++)
                {
                    names[i] = Marshal.PtrToStringAnsi((IntPtr)((i * 64) + (int)buffer));
                }
                Marshal.FreeHGlobal(buffer);
                buffer = IntPtr.Zero;

                int numPapers = DeviceCapabilities(printerName, port, DC_PAPERS, IntPtr.Zero, IntPtr.Zero);
                if (numPapers < 0)
                {
                    Console.WriteLine("No papers");
                    return 0;
                }

                buffer = Marshal.AllocHGlobal(numPapers * 2);
                numPapers = DeviceCapabilities(printerName, port, DC_PAPERS, buffer, IntPtr.Zero);
                if (numPapers < 0)
                {
                    Console.WriteLine("No papers");
                    return 0;
                }
                short[] kinds = new short[numPapers];
                for (int i = 0; i < numPapers; i++)
                {
                    kinds[i] = Marshal.ReadInt16(buffer, i * 2);
                }

                for (int i = 0; i < numPapers; i++)
                {
                    //Console.WriteLine("Paper {0} : {1}", kinds[i], names[i]);
                    if (names[i] == paperName)
                        kind = kinds[i];
                    break;
                }
            }
            finally
            {
                Marshal.FreeHGlobal(info5Ptr);
            }
            return kind;
        }


        /// <summary>
        /// 取得所有可用的纸张，並將紙張規格與名稱輸出至 console。
        /// </summary>
        /// <param name="printerName">印表機名稱。傳入 null 或空字串表示使用預設印表機。</param>
        public static void ShowPapers(string printerName)
        {
            if (String.IsNullOrEmpty(printerName))
            {
                printerName = GetDefaultPrinterName();
            }

            PRINTER_INFO_5 info5;
            int requiredSize;
            int numPrinters;
            bool foundPrinter = EnumPrintersW(PRINTER_ENUM_LOCAL | PRINTER_ENUM_CONNECTIONS,
                string.Empty, 5, IntPtr.Zero, 0, out requiredSize, out numPrinters);

            int info5Size = requiredSize;
            IntPtr info5Ptr = Marshal.AllocHGlobal(info5Size);
            IntPtr buffer = IntPtr.Zero;
            try
            {
                foundPrinter = EnumPrintersW(PRINTER_ENUM_LOCAL | PRINTER_ENUM_CONNECTIONS,
                    string.Empty, 5, info5Ptr, info5Size, out requiredSize, out numPrinters);

                string port = null;
                for (int i = 0; i < numPrinters; i++)
                {
                    info5 = (PRINTER_INFO_5)Marshal.PtrToStructure(
                        (IntPtr)((i * Marshal.SizeOf(typeof(PRINTER_INFO_5))) + (int)info5Ptr),
                        typeof(PRINTER_INFO_5));
                    if (info5.PrinterName == printerName)
                    {
                        port = info5.PortName;
                    }
                }

                int numNames = DeviceCapabilities(printerName, port, DC_PAPERNAMES, IntPtr.Zero, IntPtr.Zero);
                if (numNames < 0)
                {
                    int errorCode = GetLastError();
                    Console.WriteLine("Number of names = {1}: {0}", errorCode, numNames);
                    return;
                }

                buffer = Marshal.AllocHGlobal(numNames * 64);
                numNames = DeviceCapabilities(printerName, port, DC_PAPERNAMES, buffer, IntPtr.Zero);
                if (numNames < 0)
                {
                    int errorCode = GetLastError();
                    Console.WriteLine("Number of names = {1}: {0}", errorCode, numNames);
                    return;
                }
                string[] names = new string[numNames];
                for (int i = 0; i < numNames; i++)
                {
                    names[i] = Marshal.PtrToStringAnsi((IntPtr)((i * 64) + (int)buffer));
                }
                Marshal.FreeHGlobal(buffer);
                buffer = IntPtr.Zero;

                int numPapers = DeviceCapabilities(printerName, port, DC_PAPERS, IntPtr.Zero, IntPtr.Zero);
                if (numPapers < 0)
                {
                    Console.WriteLine("No papers");
                    return;
                }

                buffer = Marshal.AllocHGlobal(numPapers * 2);
                numPapers = DeviceCapabilities(printerName, port, DC_PAPERS, buffer, IntPtr.Zero);
                if (numPapers < 0)
                {
                    Console.WriteLine("No papers");
                    return;
                }
                short[] kinds = new short[numPapers];
                for (int i = 0; i < numPapers; i++)
                {
                    kinds[i] = Marshal.ReadInt16(buffer, i * 2);
                }

                for (int i = 0; i < numPapers; i++)
                {
                    Console.WriteLine("Paper {0} : {1}", kinds[i], names[i]);
                }
            }
            finally
            {
                Marshal.FreeHGlobal(info5Ptr);
            }
        }

        #endregion
    }
}
