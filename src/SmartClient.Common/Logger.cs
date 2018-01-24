using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.IO;
using System.Collections;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;

namespace SmartClient.Common
{
    /// <summary>
    /// 日志类型
    /// </summary>
    public enum LoggingType
    {
        /// <summary>
        /// 基本信息
        /// </summary>
        DbInfo = 1,
        /// <summary>
        /// 错误信息
        /// </summary>
        DbError = 2

    }

    /// <summary>
    /// 日志器的状态
    /// </summary>
    public enum LoggingStateEnum
    {
        /// <summary>
        /// 准备完毕，可以进行
        /// </summary>
        Ready = 0,
        /// <summary>
        /// 正在执行
        /// </summary>
        Excuting = 1
    }

    /// <summary>
    /// 写入日志的事件参数
    /// </summary>
    public sealed class LogEventArgs : EventArgs
    {
        /// <summary>
        /// 日志消息
        /// </summary>
        public string LogMessage { get; set; }

        /// <summary>
        /// 日志类型
        /// </summary>
        public LoggingType LogType { get; set; }
    }
    /// <summary>
    /// 日志记录
    /// </summary>
    public sealed class Logger
    {




        private static ReaderWriterLockSlim FileLogWriteLock = new ReaderWriterLockSlim();


        /// <summary>
        /// 最大日志文件大小（8M）
        /// </summary>
        private const int MAX_SIZE_LOG_FILE = 1024 * 1024 * 8;

        private static string _LogDir;

        /// <summary>
        /// 日志写入的路径
        /// </summary>
        private static string LogDir
        {
            get
            {


                try
                {
                    FileLogWriteLock.EnterWriteLock();


                    if (string.IsNullOrEmpty(_LogDir))
                    {


                        //首先尝试从配置中加载日志路径



                        ////如果没有配置日志路径 或者 配置的目录不存在,那么配置默认的日志路径在bin  运行目录
                        if (string.IsNullOrEmpty(_LogDir) || !Directory.Exists(_LogDir))
                        {
                            _LogDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
                            if (!Directory.Exists(_LogDir))
                            {
                                //需要检测是否存在目录，不存在，那么创建
                                Directory.CreateDirectory(_LogDir);
                            }
                        }

                        //获取配置的应用程序名称


                        //检测是否有类型目录
                        string logInfoDir = Path.Combine(_LogDir, LoggingType.DbInfo.ToString());
                        string errorDir = Path.Combine(_LogDir, LoggingType.DbError.ToString());
                        if (!Directory.Exists(logInfoDir))
                        {
                            Directory.CreateDirectory(logInfoDir);
                        }
                        if (!Directory.Exists(errorDir))
                        {
                            Directory.CreateDirectory(errorDir);
                        }

                    }
                }
                catch (Exception)
                {

                    throw;
                }
                finally
                {
                    FileLogWriteLock.ExitWriteLock();

                }


                return _LogDir;
            }

        }





        private static string _InfoLogFilePath;
        private static string _ErrorLogFilePath;
        /// <summary>
        /// 日志文件全路径
        /// </summary>
        private static string LogFilePath(LoggingType logType)
        {

            switch (logType)
            {
                case LoggingType.DbInfo:
                    //基本信息日志路径
                    if (string.IsNullOrEmpty(_InfoLogFilePath))
                    {
                        _InfoLogFilePath = GenerateFilePath(logType);
                    }
                    else
                    {

                        //当前日志文件路径不为空，那么检查大小是否越界
                        if (File.Exists(_InfoLogFilePath) && new FileInfo(_InfoLogFilePath).Length > MAX_SIZE_LOG_FILE)
                        {

                            _InfoLogFilePath = GenerateFilePath(logType);
                        }
                    }
                    return _InfoLogFilePath;
                case LoggingType.DbError:
                    //基本信息日志路径
                    if (string.IsNullOrEmpty(_ErrorLogFilePath))
                    {
                        _ErrorLogFilePath = GenerateFilePath(logType);
                    }
                    else
                    {

                        //当前日志文件路径不为空，那么检查大小是否越界
                        if (File.Exists(_ErrorLogFilePath) && new FileInfo(_ErrorLogFilePath).Length > MAX_SIZE_LOG_FILE)
                        {

                            _ErrorLogFilePath = GenerateFilePath(logType);
                        }
                    }
                    return _ErrorLogFilePath;
                default:
                    throw new Exception("未能识别的日志输出类型！");

            }





        }

        /// <summary>
        /// 消息队列
        /// </summary>
        private static Queue<LogEventArgs> _queueOfMessage = null;


        /// <summary>
        /// 当前日志器的状态
        /// </summary>
        private static LoggingStateEnum _loggingStatus;


        /// <summary>
        /// 触发写入日志的事件
        /// </summary>
        private static event EventHandler<LogEventArgs> OnStartWriteLog;

        static Logger()
        {
            _queueOfMessage = new Queue<LogEventArgs>();
            _loggingStatus = LoggingStateEnum.Ready;

            //注册处理委托
            OnStartWriteLog += LoggerOnStartWriteLog;
        }


        /// <summary>
        /// 进行日志写入的时候 触发的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void LoggerOnStartWriteLog(object sender, LogEventArgs e)
        {
            //处理写入日志中
            _queueOfMessage.Enqueue(e);

            if (_loggingStatus != LoggingStateEnum.Excuting)
            {

                Task.Factory.StartNew(() =>
                {

                    //激活  日志队列处理器，进行队列的处理

                    _loggingStatus = LoggingStateEnum.Excuting;

                    while (_queueOfMessage.Count > 0)
                    {
                        var messageContaienr = _queueOfMessage.Dequeue();
                        WriteMessageToLog(messageContaienr);
                    }

                    //写入完毕后 更改状态
                    _loggingStatus = LoggingStateEnum.Ready;



                });



            }
        }

        /// <summary>
        /// 写入异常信息
        /// </summary>
        /// <param name="ex"></param>
        public static void WriteException(Exception ex)
        {
            var agrs = new LogEventArgs() { LogMessage = ex.ToString(), LogType = LoggingType.DbError };
            WriteToLog(agrs);
        }

        /// <summary>
        /// 写入日志内容
        /// </summary>
        /// <param name="content"></param>
        public static void WriteToLog(LogEventArgs agrs)
        {

            OnStartWriteLog.Invoke(null, agrs);

        }
        /// <summary>
        /// 写入日志内容到文件
        /// </summary>
        /// <param name="contentContaienr">日志消息容器</param>
        private static void WriteMessageToLog(LogEventArgs contentContaienr)
        {
            var logType = contentContaienr.LogType;
            string content = contentContaienr.LogMessage;
            var logFile = LogFilePath(logType);
            WriteLogContentToFile(logFile, content);
        }


        /// <summary>
        /// 写入日志内容到文件
        /// </summary>
        /// <param name="fileFullPath"></param>
        /// <param name="content"></param>
        private static void WriteLogContentToFile(string fileFullPath, string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                return;
            }
            try
            {
                FileLogWriteLock.EnterWriteLock();


                //内容格式化
                content = FormatLogContent(content);

                bool isAppend = false;


                if (File.Exists(fileFullPath))
                {
                    isAppend = true;
                }


                //内容追加
                if (isAppend == true)
                {


                    using (var fs = new FileStream(fileFullPath, FileMode.Append))
                    {
                        using (var sr = new StreamWriter(fs))
                        {
                            sr.Write(content);
                        }
                    }

                }
                else
                {
                    //内容创建

                    using (var fs = new FileStream(fileFullPath, FileMode.CreateNew))
                    {
                        using (var sr = new StreamWriter(fs))
                        {
                            sr.Write(content);
                        }
                    }
                }

            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                FileLogWriteLock.ExitWriteLock();
            }


        }

        /// <summary>
        /// 格式化 消息内容
        /// </summary>
        /// <param name="content"></param>
        private static string FormatLogContent(string content)
        {
            StringBuilder sb = new StringBuilder();
            var charOfNewLine = Environment.NewLine;
            sb.Append("-----------------------------------begin--------------------------------");
            sb.Append(charOfNewLine);
            sb.Append(content);
            sb.Append(charOfNewLine);
            sb.AppendFormat("Excute DateTime：{0}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff"));
            sb.Append(charOfNewLine);
            sb.Append("-----------------------------------end--------------------------------");
            sb.Append(charOfNewLine);

            return sb.ToString();
        }

        /// <summary>
        /// 生成日志文件全路径
        /// yyyy_MM_dd_HH_mm
        /// </summary>
        /// <param name="logType"></param>
        /// <returns></returns>
        private static string GenerateFilePath(LoggingType logType)
        {



            string filePath = string.Empty;


            var token = DateTime.Now.ToString("yyyy_MM_dd_HH_mm");// + "_" + Guid.NewGuid().ToString().Split('-')[0];
            var logTypeDir = logType.ToString();
            filePath = string.Format("{0}\\{1}\\{2}.log", LogDir, logTypeDir, token);


            if (string.IsNullOrEmpty(_InfoLogFilePath))
            {
                _InfoLogFilePath = filePath;
            }


            //当前日志文件路径不为空，那么检查大小是否越界
            var fi = new FileInfo(_InfoLogFilePath);

            if (File.Exists(_InfoLogFilePath) //文件存在
                && fi.Length > MAX_SIZE_LOG_FILE)//超过最大文件上限
            {

                if (fi.CreationTime.AddMinutes(1) < DateTime.Now)
                {
                    //一旦上次创建的文件超过1min  那么不再使用上一分钟的文件名
                    _InfoLogFilePath = filePath;
                }
                else
                {
                    //毫秒追加
                    string miniFileName = string.Concat(Path.GetFileNameWithoutExtension(fi.FullName), "_", DateTime.Now.Millisecond, ".log");

                    _InfoLogFilePath = Path.Combine(fi.DirectoryName, miniFileName);
                }

            }



            return _InfoLogFilePath;



        }

    }

}
