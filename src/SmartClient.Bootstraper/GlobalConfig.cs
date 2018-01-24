using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartClient.Bootstraper
{
    internal static  class GlobalConfig
    {
        //端口号
        public const int APP_PORT = 6670;
        /// <summary>
        /// 程序基本绑定地址。用来将本地的localhost .127.0.0.1绑定监听
        /// </summary>
        public static string BaseBindingAddress = string.Format("http://+:{0}", APP_PORT);//绑定所有IP的6670端口
        /// <summary>
        /// 程序内部通信使用基于本地的地址
        /// </summary>
        public static string BaseAddress = string.Format("http://127.0.0.1:{0}", APP_PORT);


    }
}
