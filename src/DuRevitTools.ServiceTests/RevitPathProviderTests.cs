using Microsoft.VisualStudio.TestTools.UnitTesting;
using DuRevitTools.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuRevitTools.Service.Tests
{
    [TestClass()]
    public class RevitPathProviderTests
    {
        [TestMethod()]
        public void GetInstallVersionTest()
        {
            var versions = RevitPathProvider.GetInstallVersion();

            Assert.IsNotNull(versions.Count() == 1);

            var version = versions.FirstOrDefault();

            Assert.IsNotNull(version);

            Assert.IsTrue(System.IO.Directory.Exists(version.InstallLocation));
        }
    }
}