using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestHelper;

namespace DuRevitTools.APIAnalyzer.Test
{
    [TestClass]
    public class XMLDocUnitTest:CodeFixVerifier
    {
        [TestMethod]
        public void PropertyTest()
        {

        }

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new XMLDocCodeFixProvider();
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new XMLDocAnalyzer();
        }
    }
}
