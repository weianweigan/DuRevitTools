using Microsoft.VisualStudio.TestTools.UnitTesting;
using DuRevitTools.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace DuRevitTools.Model.Tests
{
    [TestClass()]
    public class DocProviderTests
    {
        [TestMethod()]
        public void GetMemeberTest()
        {
            var member = DocProvider.Provider.GetMemeber("Autodesk.Revit.ApplicationServices.Application");
            Assert.IsNotNull(member);
        }
    }
}