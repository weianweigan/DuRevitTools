using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DuRevitTools.DocGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            var doc = HelpProvider.GetRevitApiDocAsync(@"C:\Autodesk\Revit_2018_G1_Win_64bit_dlm\Utilities\SDK\Software Development Kit\RevitAPI\html").GetAwaiter().GetResult();

            string xmlStr = HelpProvider.GetXml(doc);

            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\RevitAPI.xml";
            System.IO.File.WriteAllText(path, xmlStr);
        }
    }
}
