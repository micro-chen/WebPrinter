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

namespace SmartClient.Web.Controllers
{
    /// <summary>
    /// 客户端的UAC后的控制总线
    /// </summary>
    public class WorkBenchController  : BaseApiController
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
        /// 启动菜鸟组件
        /// 0失败 ，1 成功
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IMessageConteiner StartCaiNiaoPrinter()
        {

            var dataContainer = new MessageConteiner<int>();


            var result = 0;
            try
            {

                result = SystemAppExtension.SatrtCaiNiaoPrinter();
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
        /// 获取客户端本地的菜鸟安装信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IMessageConteiner GetCaiNiaoInstallPath()
        {
            var dataContainer = new MessageConteiner<SoftwareInfo>();

            SoftwareInfo model;
            var result = SystemAppExtension.IsCaiNiaoPrintInstalled(out model);
            if (result == false)
            {
                //如果机器没有安装菜鸟组件 那么返回内置组件信息
                model = new SoftwareInfo {
                    DisplayName = "内置菜鸟打印组件",
                    InstallLocation = SystemAppExtension.DefaultInnerCaiNiaoInstallPath,
                    Version = SystemAppExtension.DefaultInnerCaiNiaoVersion };
            }

            dataContainer.Data = model;
            return dataContainer;

        }

       

        /// <summary>
        /// 重启菜鸟组件
        /// 0失败 ，1 成功
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IMessageConteiner RestartCaiNiaoPrinter()
        {

            var dataContainer = new MessageConteiner<int>();


            var result = 0;
            try
            {

                result = SystemAppExtension.RestartCaiNiaoPrinter();
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
        /// 关闭菜鸟组件
        /// 0失败 ，1 成功
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IMessageConteiner CloseCaiNiaoPrinter()
        {

            var dataContainer = new MessageConteiner<int>();


            var result = 0;
            try
            {

                result = SystemAppExtension.TerminalCaiNiaoPrinter();
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
        /// 设置默认打印机
        /// 0失败 ，1 成功
        /// {"printername":"PDF-XChange Printer 2012"}
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IMessageConteiner SetDefaultPrinter(SetDefaultPrinterViewModel agrs)
        {

            var dataContainer = new MessageConteiner<int>();


            var result = 0;
            try
            {
                if (null == agrs || string.IsNullOrEmpty(agrs.PrinterName))
                {
                    throw new BusinessException("打印机名称不能为空！");
                }
                result = SystemAppExtension.SetDefaultPrinterByName(agrs.PrinterName);
            }
            catch (BusinessException ex)
            {

                dataContainer.Message = ex.Message;
                dataContainer.Data = 0;
            }

            dataContainer.Data = result;

            return dataContainer;

        }

    }
}