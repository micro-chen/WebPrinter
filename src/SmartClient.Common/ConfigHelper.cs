using System;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Linq;

namespace SmartClient.Common
{
    public class ConfigHelper
    {

        /// <summary>
        /// 读取配置文件某项的值
        /// </summary>
        /// <param name="key">appSettings的key</param>
        /// <param name="config">特定的配置对象</param>
        /// <returns>appSettings的Value</returns>
        public static string GetConfig(string key, Configuration config = null)
        {
            string _value = string.Empty;
            if (null == config)
            {
                if (ConfigurationManager.AppSettings[key] != null)
                {
                    _value = ConfigurationManager.AppSettings[key];
                }
            }
            else
            {
                if (config.AppSettings.Settings.AllKeys.Contains(key))
                {
                    _value = config.AppSettings.Settings[key].Value;
                }
            }

            return _value;
        }



        /// <summary>
        /// 从指定的文件读取config对象
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static Configuration GetConfigObjectFromFile(string filePath, string key)
        {
            Configuration config = null;
            if (!System.IO.File.Exists(filePath))
            {
                throw new FileNotFoundException("配置文件不存在！请检查路径："+filePath??"");
            }
            ExeConfigurationFileMap filemap = new ExeConfigurationFileMap();
            filemap.ExeConfigFilename = filePath;
            config = ConfigurationManager.OpenMappedExeConfiguration(filemap, ConfigurationUserLevel.None);
            return config;
        }

        /// <summary>
        /// 读取配置文件某项的值
        /// </summary>
        /// <param name="key">appSettings的key</param>
        /// <returns>appSettings的Value</returns>
        public static string GetConfigFromConfigFile(string filePath, string key)
        {
            string _value = string.Empty;
            if (System.IO.File.Exists(filePath))
            {
                ExeConfigurationFileMap filemap = new ExeConfigurationFileMap();
                filemap.ExeConfigFilename = filePath;
                var config = ConfigurationManager.OpenMappedExeConfiguration(filemap, ConfigurationUserLevel.None);


                if (config.AppSettings.Settings.AllKeys.Contains(key))
                {
                    _value = config.AppSettings.Settings[key].Value;
                }
            }



            return _value;
        }


        /// <summary>
        /// 得到AppSettings中的配置Bool信息
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool GetConfigBool(string key, Configuration config = null)
        {
            bool result = false;
            string cfgVal = GetConfig(key, config);
            if (null != cfgVal && string.Empty != cfgVal)
            {
                try
                {
                    result = bool.Parse(cfgVal.ToLower().Trim());
                }
                catch (FormatException)
                {
                    // Ignore format exceptions.
                }
            }
            return result;
        }
        /// <summary>
        /// 得到AppSettings中的配置Decimal信息
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static decimal GetConfigDecimal(string key, Configuration config = null)
        {
            decimal result = 0;
            string cfgVal = GetConfig(key,config);
            if (null != cfgVal && string.Empty != cfgVal)
            {
                try
                {
                    result = decimal.Parse(cfgVal);
                }
                catch (FormatException)
                {
                    // Ignore format exceptions.
                }
            }

            return result;
        }
        /// <summary>
        /// 得到AppSettings中的配置int信息
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static int GetConfigInt(string key, Configuration config = null)
        {
            int result = 0;
            string cfgVal = GetConfig(key,config);
            if (null != cfgVal && string.Empty != cfgVal)
            {
                try
                {
                    result = int.Parse(cfgVal);
                }
                catch (FormatException)
                {
                    // Ignore format exceptions.
                }
            }

            return result;
        }



    }
}
