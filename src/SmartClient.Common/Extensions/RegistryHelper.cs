using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;

/*读注册表：
   RegistryHelper rh = new RegistryHelper();

   string portName = rh.GetRegistryData(Registry.LocalMachine, "SOFTWARE\\TagReceiver\\Params\\SerialPort", "PortName");
   写注册表：
   RegistryHelper rh = new RegistryHelper();
   rh.SetRegistryData(Registry.LocalMachine, "SOFTWARE\\TagReceiver\\Params\\SerialPort", "PortName", portName);*/

namespace SmartClient.Common.Extensions
{

    /// <summary>
    /// 注册表辅助


    /// </summary>
    public class RegistryHelper
    {
        /// <summary>
        /// 读取指定名称的注册表的值
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetRegistryData(RegistryKey root, string subkey, string name)
        {
            string registData = "";
            RegistryKey myKey = root.OpenSubKey(subkey, true);
            if (myKey != null)
            {
                registData = myKey.GetValue(name).ToString();
            }

            return registData;
        }

        /// <summary>
        /// 向注册表中写数据
        /// </summary>
        /// <param name="name"></param>
        /// <param name="tovalue"></param> 
        public static bool SetRegistryData(RegistryKey root, string subkey, string name, string value)
        {

            var result = false;
            try
            {
                RegistryKey aimdir = root.CreateSubKey(subkey);
                aimdir.SetValue(name, value);
                result = true;
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return result;

        }

        /// <summary>
        /// 删除注册表中指定的注册表项
        /// </summary>
        /// <param name="name"></param>
        public static bool DeleteRegist(RegistryKey root, string subkey, string name)
        {
            var result = false;
            try
            {

                string[] subkeyNames;
                RegistryKey myKey = root.OpenSubKey(subkey, true);
                subkeyNames = myKey.GetValueNames();//GetSubKeyNames();
                foreach (string aimKey in subkeyNames)
                {
                    if (aimKey == name)
                        myKey.DeleteValue(name);//DeleteSubKeyTree(name);
                }

                result = true;
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return result;
        }

        /// <summary>
        /// 判断指定注册表项是否存在
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool IsRegistryExist(RegistryKey root, string subkey, string name)
        {
            bool _exit = false;
            string[] subkeyNames;
            RegistryKey myKey = root.OpenSubKey(subkey, true);

            if (null==myKey)
            {
                throw new Exception(string.Format("未能打开指定的注册表；root {0},key : {1} ,item:{2}.", root.Name,subkey,name));

            }

            subkeyNames = myKey.GetValueNames();//.GetSubKeyNames();
            foreach (string keyName in subkeyNames)
            {
                if (keyName == name)
                {
                    _exit = true;
                    return _exit;
                }
            }

            return _exit;
        }

    }
}
