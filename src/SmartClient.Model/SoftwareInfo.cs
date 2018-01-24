using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SmartClient.Model
{
    /// <summary>
    /// 客户端安装的软件信息
    /// </summary>
    public class SoftwareInfo
    {
        /// <summary>
        /// 显示的名称
        /// </summary>
        public string DisplayName { get; set; }
        public string DisplayVersion { get; set; }
        public string InstallDate { get; set; }
        public string Publisher { get; set; }

        /// <summary>
        /// 安装路径
        /// </summary>
        public string InstallLocation { get; set; }
        public string Version { get; set; }
        public string VersionMinor { get; set; }
        public string VersionMajor { get; set; }
        public string EstimatedSize { get; set; }
        public string UninstallString { get; set; }
        public string HelpLink { get; set; }
    }
}