using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SmartClient.Model;
using SmartClient.Common;
using SmartClient.Web.ViewModel;
using SmartClient.Common.Extensions;
using SmartClient.Web.Common;

namespace SmartClient.Web.Controllers
{

    /// <summary>
    /// 系统控制器
    /// </summary>
    public class SystemController : BaseApiController
    {
        //-----------------------------注意 这方法不能写到基础的控制器中，会出现404找不到Action---------begin----------------
        /// <summary>
        /// 保持激活
        /// </summary>
        /// <returns></returns>
        // GET api/System/KeepAlive/
        [HttpGet]
        public override IMessageConteiner KeepAlive()
        {
            var dataContainer = base.KeepAlive();
            return dataContainer;
        }
        /// <summary>
        /// 获取软件版本信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public override IMessageConteiner GetVersionInfo()
        {
            var dataContainer = base.GetVersionInfo();
            return dataContainer;

        }
        /// <summary>
        /// 检索菜鸟打印进程是否进行中
        /// 0 未运行 ，1 运行中
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public override IMessageConteiner CheckCaiNiaoPrinterStatus()
        {
            var dataContainer = base.CheckCaiNiaoPrinterStatus();

            return dataContainer;

        }

        //-----------------------------注意 这方法不能写到基础的控制器中，会出现404找不到Action---------end----------------

        /// <summary>
        /// 工作台是否运行中
        /// 0失败 ，1 成功
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IMessageConteiner IsWorkBenchIsRunning()
        {

            var dataContainer = new MessageConteiner<int>();


            var result = 0;
            try
            {

                result = SystemAppExtension.IsWorkBenchIsRunning();
            }
            catch (BusinessException ex)
            {

                dataContainer.Message = ex.Message;
                
                dataContainer.Data = 0;
            }

            dataContainer.Data = result;

            return dataContainer;

        }


        /// <summary>
        /// 启动工作台
        /// 0失败 ，1 成功
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IMessageConteiner StartWorkBench()
        {

            var dataContainer = new MessageConteiner<int>();


            var result = 0;
            try
            {

                result = SystemAppExtension.StartWorkBench();
            }
            catch (BusinessException ex)
            {

                dataContainer.Message = ex.Message;
                
                dataContainer.Data = 0;
            }

            dataContainer.Data = result;

            return dataContainer;

        }

        /// <summary>
        /// 接收打印任务消息（压缩过的json二进制数据）
        /// 返回插入后的任务号
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IMessageConteiner ReceiveTaskMessage(TaskMessageViewModel args)
        {

            var dataContainer = new MessageConteiner<string>();


            var result = string.Empty;
            try
            {

                result = TaskQueueManager.Current.AddMessageToQueue(args.Message);
            }
            catch (BusinessException ex)
            {

                dataContainer.Message = ex.Message;
                
                dataContainer.Data = "";
            }

            dataContainer.Data = result;

            return dataContainer;

        }



    }
}
