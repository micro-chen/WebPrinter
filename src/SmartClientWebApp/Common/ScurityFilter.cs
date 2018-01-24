using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Cors;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

using SmartClient;
using SmartClient.Model;
using SmartClient.Common.Encrypt;
using SmartClient.Common.Extensions;

namespace SmartClient.Web.Common
{
    /// <summary>
    /// 安全验证过滤器--------IE6----------不支持在头部，自定义Http 头，所以，不进行验证了
    /// 提供基本的安全验证，验证来自请求的token ，token  规则可根据需要进行自定义
    /// </summary>
    public class ScurityFilter : ActionFilterAttribute
    {
        /// <summary>
        /// 是否为正确的token
        /// </summary>
        private readonly string IsValidToken = "success";
        /// <summary>
        /// 客户端请求的头验证表示
        /// </summary>
        public static string IsFromClientToken { get { return "console@smartclient.com"; } }

        /// <summary>
        /// 安全验证过滤器，进行安全校验错误结果
        /// </summary>
        private static readonly IMessageConteiner ERROR_MESSAGE = new MessageConteiner<string>
        {
            Status = MessageStatus.Error,
            Message = ExceptionHashTable.Fobidden.GetDescription()
        };


        /// <summary>
        /// Action 执行时候的注入
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuting(HttpActionContext filterContext)
        {

            #region 无效代码


            //#if DEBUG
            //            //本地调试模式不启动过滤筛选
            //            base.OnActionExecuting(filterContext);
            //            return;
            //#endif
            ////当Action执行进入前，获取Action的描述+参数信息
            //var actionDescrip = filterContext.ActionDescriptor;
            //if (null == actionDescrip)
            //{
            //    base.OnActionExecuting(filterContext);
            //    return;
            //}
            //var controllerName = actionDescrip.ControllerDescriptor.ControllerName;
            //var actionName = actionDescrip.ActionName;

            //var paras = HttpContext.Current.Request.Params;
            ////part1 查询参数
            ////part2 表单参数
            //var sb_paras_Content = new StringBuilder();
            //foreach (var key in paras.Keys)
            //{
            //    var item = paras.GetValues(key.ToString());
            //    sb_paras_Content.AppendFormat("{0}:{1}", key.ToString(), item[0].ToString()).Append("\r\n");
            //}


            //var logContent = string.Format(actionLogTemplate, controllerName, actionName, sb_paras_Content.ToString());
            //Logger.WriteToLog(logContent);
            //string origin = string.Empty;
            //if (filterContext.Request.Headers.Contains(CorsConstants.Origin))
            //{
            //    origin = filterContext.Request.Headers.GetValues(CorsConstants.Origin).FirstOrDefault();
            //}



            //if (String.IsNullOrEmpty(origin))
            //{
            //    //尝试从 Http Referer中获取
            //    if (filterContext.Request.Headers.Contains(CorsConstants.Referer))
            //    {
            //        string refer = filterContext.Request.Headers.GetValues(CorsConstants.Referer).FirstOrDefault();
            //        if (!string.IsNullOrEmpty(refer))
            //        {
            //            origin = CorsConstants.GetUriAddress(refer);
            //        }
            //    }

            //}

            //if (String.IsNullOrEmpty(origin))
            //{
            //    //尝试从 Http Host 中获取
            //    //注意：这里其实是一个放开权限，如果请求拦截不到 Orgin 和Refer 中的地址，那么直接放行
            //    //这里有危险，因为相当于验证失效
            //    string requestUrl = filterContext.Request.RequestUri.OriginalString;
            //    origin = CorsConstants.GetUriAddress(requestUrl);

            //}

            ////验证orgin,不在范围内的不给请求
            //if (Startup.AuthedDomains.Contains(origin))
            //{
            //    base.OnActionExecuting(filterContext);
            //}
            //else
            //{
            //    //不能执行Action 直接返回错误结果
            //    //在action执行前终止请求时，应该使用填充方法Response，将不返回action方法体。  
            //    //注意  我们将结果返回一个对象，我们在WEBAPI启动的时候，对处理的formattors进行了设定，仅仅json的处理格式化
            //    filterContext.Response = filterContext.Request.CreateResponse(HttpStatusCode.OK, ERROR_MESSAGE);

            //}

            #endregion


            string fromHeader = string.Empty;
            try
            {
                fromHeader = filterContext.Request.Headers.GetValues(CorsConstants.From).FirstOrDefault();
            }
            catch { }
            //控制台启动标志 不验证
            if (!string.IsNullOrEmpty(fromHeader)&&fromHeader==IsFromClientToken)
            {
                base.OnActionExecuting(filterContext);
                return;
            }

            //验证请求的Token
            string encyptedToken = string.Empty;
            try
            {
                encyptedToken = filterContext.Request.Headers.GetValues(CorsConstants.Token).FirstOrDefault();
            }
            catch { }

            string token = string.Empty;
            if (!string.IsNullOrEmpty(encyptedToken))
            {
                token = DESEncrypt.Decrypt(encyptedToken);
            }
            //验证orgin,不在范围内的不给请求
            if (token == IsValidToken)
            {
                base.OnActionExecuting(filterContext);
            }
            else
            {
                //不能执行Action 直接返回错误结果
                //在action执行前终止请求时，应该使用填充方法Response，将不返回action方法体。  
                //注意  我们将结果返回一个对象，我们在WEBAPI启动的时候，对处理的formattors进行了设定，仅仅json的处理格式化
                filterContext.Response = filterContext.Request.CreateResponse(HttpStatusCode.OK, ERROR_MESSAGE);

            }

        }

    }
}