using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartClient;
using SmartClient.Common.Extensions;
using SmartClient.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartClient.Tests
{
    [TestClass()]
    public class SystemAppExtensionTests
    {
        [TestMethod()]
        public void IsCaiNiaoPrintInstalledTest()
        {

            SoftwareInfo model;

            var result = SystemAppExtension.IsCaiNiaoPrintInstalled(out model);

            string innerCainNiao = SystemAppExtension.DefaultInnerCaiNiaoInstallPath;
            Assert.IsTrue(result);

        }



        [TestMethod()]
        public void CheckCaiNiaoPrinterStatusTest()
        {
            var result = SystemAppExtension.CheckCaiNiaoPrinterStatus();

            Assert.IsTrue(result==1);
        }
    }
}