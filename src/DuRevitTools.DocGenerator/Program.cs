using DuRevitTools.Model;
using System;
using System.Collections.Generic;
using System.Data.Common;
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

            //TODO 分为两个文件 RevitUI.xml  RevitAPIUI.xml

            var docUIMemeber = doc.members.Where(p => p.name.Contains("Autodesk.Revit.UI"));

            var uiDoc = CloneToUIDoc(doc, docUIMemeber);

            var revitUIMemebr = doc.members.Where(p => p.name.Contains("Autodesk.Revit.UI"));
            doc.members = revitUIMemebr.ToList();


            string xmlStr = HelpProvider.GetXml(doc);
            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\RevitAPI.xml";
            System.IO.File.WriteAllText(path, xmlStr);

            string xmlUIStr = HelpProvider.GetXml(uiDoc);
            string UIpath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\RevitAPIUI.xml";
            System.IO.File.WriteAllText(UIpath, xmlUIStr);
        }

        private static doc CloneToUIDoc(doc doc, IEnumerable<member> docUIMemeber)
        {
            doc = new doc()
            {
                assembly =new assembly() { name = "RevitAPIUI" }
            };

            doc.members = docUIMemeber.ToList();

            return doc;
        }
    }
}
