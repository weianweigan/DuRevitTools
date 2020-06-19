using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Xml;
using System.IO;
using System.Xml.Serialization;

namespace GStarGenerator
{
    public class HelpProvider
    {
        private const string dllName = "RevitApi";

        public static doc GetRevitApiDoc(string folder)
        {
            doc doc = new doc();

            doc.assembly = new assembly() { name = dllName};

            var files = GetFolderFiles(folder).Take(2);

            var tasks = new List<Task<helpHtmlPraser>>();

            foreach (var item in files)
            {
                helpHtmlPraser praser = new helpHtmlPraser(item);

                var task = praser.InitAsync();

                tasks.Add(task);
            }

            Task.WaitAll(tasks.ToArray());

            foreach (var item in tasks)
            {
                var data = item.Result;

                doc.members.Add(data.Member);
            }

            return doc;
        }

        public static async Task<doc> GetRevitApiDocAsync(string folder)
        {
            doc doc = new doc();

            doc.assembly = new assembly() { name = dllName };

            var files = GetFolderFiles(folder);

            var tasks = new List<Task<helpHtmlPraser>>();

            int i = 0;

            foreach (var item in files)
            {
                helpHtmlPraser praser = new helpHtmlPraser(item);

                var data = await praser.InitAsync();

                if (data.IsMember)
                {
                    doc.members.Add(data.Member);
                    Console.WriteLine($"Num:{i}/{files.Length} -- File: {item}");

                }
                else
                {
                    Console.WriteLine($"Num:{i}/{files.Length} -- File: {item}");

                }
                i++;
                //if (i>2)
                //{
                //    break;
                //}
            }

            return doc;
        }

        internal static string GetXml(doc doc)
        {
            //&lt; see cref = "T:Autodesk.Revit.DB.XYZ" / &gt;
            return Serializer(typeof(doc), doc).Replace("&lt;","<").Replace("/&gt;","/>");       
        }

        public static string Serializer(Type type, object obj)
        {
            MemoryStream Stream = new MemoryStream();
            XmlSerializer xml = new XmlSerializer(type);
            //序列化对象  
            xml.Serialize(Stream, obj);
            Stream.Position = 0;
            StreamReader sr = new StreamReader(Stream);
            string str = sr.ReadToEnd();

            sr.Dispose();
            Stream.Dispose();

            return str;
        }

        private static string[] GetFolderFiles(string folder)
        {
            return System.IO.Directory.GetFiles(folder);
        }
    }
}
