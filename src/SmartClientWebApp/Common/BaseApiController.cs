
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using SmartClient;
using SmartClient.Common;
using SmartClient.Web;
using SmartClient.Model;
using SmartClient.Common.Extensions;

public class BaseApiController : ApiController
{

    /// <summary>
    /// 保持激活
    /// </summary>
    public virtual IMessageConteiner KeepAlive()
    {
        var dataContainer = new MessageConteiner<string>();
        dataContainer.Message = DateTime.Now.ToString();
        return dataContainer;
    }
    /// <summary>
    /// 获取软件版本信息
    /// </summary>
    /// <returns></returns>
    public virtual IMessageConteiner GetVersionInfo()
    {
        var dataContainer = new MessageConteiner<VersionInfo>();

        var vObj = VersionInfo.CurrentVersion;
        dataContainer.Data = vObj;

        return dataContainer;

    }


    /// <summary>
    /// 检索菜鸟打印进程是否进行中
    /// 0 未运行 ，1 运行中
    /// </summary>
    /// <returns></returns>
  
    public virtual  IMessageConteiner CheckCaiNiaoPrinterStatus()
    {
        var dataContainer = new MessageConteiner<int>();


        var result = 0;
        try
        {
            result = SystemAppExtension.CheckCaiNiaoPrinterStatus();
        }
        catch (BusinessException ex)
        {
            SmartClient.Common.Logger.Error(ex);

            dataContainer.Message = ex.Message;
            dataContainer.Data = 0;
        }

        dataContainer.Data = result;

        return dataContainer;

    }





}

