using Xunit;
using DuRevitTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuRevitTools.Tests
{
    public class RevitToolsViewModelTests
    {
        [Fact()]
        public void AddinPathClickTest()
        {
            RevitToolsViewModel vm = new RevitToolsViewModel();

            vm.AddinPathClick();

            Assert.True(true);
        }

        [Fact]
        public void VersionsTest()
        {
            RevitToolsViewModel vm = new RevitToolsViewModel();

            var versions = vm.RevitVersions;

            Assert.True(versions.First() == "2018");
        }

        [Fact]
        public void GetRevitProcessTest()
        {
            RevitToolsViewModel vm = new RevitToolsViewModel();

            var ptr = vm.GetRevitProcessPtr();

            Assert.NotNull(ptr);
        }
    }
}