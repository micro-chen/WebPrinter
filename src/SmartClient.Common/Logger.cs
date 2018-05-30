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
using log4net;
using log4net.Config;

namespace SmartClient.Common
{

    /// <summary>
    /// 提供log4的日志
    /// </summary>
    public static class Logger
    {
        private static ILog _logWriter = log4net.LogManager.GetLogger("SmartClient.Common"); 

        /// <summary>
        /// 静态构造函数
        /// 在使用类的时候 进行一次配置
        /// </summary>
         static Logger()
        {
            Logger.LoadConfig();
        }
        /// <summary>
        /// 是否输出日志
        /// </summary>
        private static bool IsOutPutLog
        {
            get
            {
                //var configValue = ConfigHelper.GetConfigBool("IsOutPutLog");
                var configValue = ConfigHelper.GetConfigFromConfigFile(GlobalConfig.SettingsConfigFilePath, "IsOutPutLog");
                if (string.IsNullOrEmpty(configValue))
                {
                    return false;
                }
                
                return configValue.ToBool();
            }
        }

        /// <summary>
        /// log4的配置文件路径
        /// </summary>
        public static string ConfigFilePath
        {
            get
            {
                var log4ConfigFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs", "log4net.config");
                if (!File.Exists(log4ConfigFile))
                {
                    throw new Exception("未能找到 log4net 的配置文件！在路径：" + log4ConfigFile);
                }

                return log4ConfigFile;
            }
        }

        /// <summary>
        /// 初始化log4
        /// </summary>
        public static void LoadConfig()
        {
            //配置log4
            XmlConfigurator.ConfigureAndWatch(new FileInfo(ConfigFilePath));
        }
        /// <summary>
        /// info 级别的log
        /// </summary>
        /// <param name="msg"></param>
        public static void Info(string msg)
        {
            if (!IsOutPutLog)
            {
                return;
            }

            _logWriter.Info(msg);

        }

        /// <summary>
        ///  Warn 级别的log
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="title"></param>
        public static void Warn(Exception ex, string title = "Warn")
        {
            if (!IsOutPutLog)
            {
                return;
            }

            _logWriter.Warn(title, ex);

        }

        public static void Error(string exMsg, string title = "Error")
        {
            if (!IsOutPutLog)
            {
                return;
            }
            Error(new Exception(exMsg));
        }
        /// <summary>
        /// Error 级别的log
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="title"></param>

        public static void Error(Exception ex, string title = "Error")
        {
            if (!IsOutPutLog)
            {
                return;
            }

            _logWriter.Error(title, ex);

        }



    }
}
