using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace DuRevitTools.APIAnalyzer
{
    public static class XmlDocHelper
    {
        private const string RevitAPIXMLDoc = "RevitAPI.xml";

        ///<summary>Whether Xml doc file existe</summary>
        public static bool IsXmlDocExist(string display)
        {
            var filePath = Path.Combine(Path.GetDirectoryName(display), RevitAPIXMLDoc);
            return File.Exists(filePath);
        }

        ///<summary>Generate RevitAPI.xml for RevitAPI.dll</summary>
        public static void AddXMLFileToRef(string metaDataPath)
        {
            var filePath = Path.Combine(Path.GetDirectoryName(metaDataPath), RevitAPIXMLDoc);
            if (!File.Exists(filePath))
            {
                File.WriteAllText(filePath, XMLDocAnalyzer.DocProvider.XMLDoc);
            }
        }

        ///<summary>async to Generate RevitAPI.xml for RevitAPI.dll</summary>
        public static async Task AddXMLFileToRefAsync(string metaDataPath)
        {
            await Task.Run(() =>
            AddXMLFileToRef(metaDataPath));
        }
    }
}
