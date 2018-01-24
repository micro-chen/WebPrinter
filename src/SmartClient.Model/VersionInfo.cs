using System;


namespace SmartClient.Model
{
    /// <summary>
    /// 应用版本信息
    /// 保持跟菜鸟同步发行
    /// </summary>
    public class VersionInfo
    {
        public VersionInfo()
        {
            this.AppName = "SmartClient";
            this.VersionNumber = "1.0";
            this.AdaptCaiNiaoVersionNumber = " 0.2.8.2";
        }

        private static VersionInfo _CurrentVersion;
        /// <summary>
        /// 当前版本的实例-单例模式
        /// </summary>
        public static VersionInfo CurrentVersion
        {
            get
            {
                if (null== _CurrentVersion)
                {
                    _CurrentVersion = new VersionInfo();
                }
                return _CurrentVersion;
            }
        }
        /// <summary>
        /// 应用名称
        /// </summary>
        public string AppName { get; private set; }

        /// <summary>
        /// 版本号
        /// </summary>
        public string VersionNumber { get; private set; }

       

         /// <summary>
        /// 适配菜鸟版本号
        /// </summary>
        public string AdaptCaiNiaoVersionNumber { get; private set; }
    }
}