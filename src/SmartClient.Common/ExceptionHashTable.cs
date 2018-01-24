using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace SmartClient
{
    /// <summary>
    /// 系统故障错误码，状态值，枚举
    /// </summary>
    public enum ExceptionHashTable
    {
        [Description("4033---禁止访问，服务器收到请求，但是拒绝提供服务！无权访问资源！")]
        Fobidden = 4033,
        [Description("5001 - 服务器遇到错误，无法完成请求!系统处理故障！")]
        InnerError = 5001
    }

}