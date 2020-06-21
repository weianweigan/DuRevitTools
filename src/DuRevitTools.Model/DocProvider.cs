
/*
 * Create 2020/6/20
 * WeiGan
 * 
 * Provider for RevitAPI Document
 * 
 */

using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Xml.Serialization;

namespace DuRevitTools.Model
{
    ///<summary>Get Doc Resources form this class</summary>
    public class DocProvider
    {
        private static DocProvider _provider;

        private DocProvider()
        {
            InitDoc();
        }

        #region Properties

        ///<summary>Doc for RevitAPI</summary>
        public doc Doc { get; private set; }

        ///<summary>Default Doc for RevitAPI</summary>
        public static doc DefaultDoc => Provider.Doc;

        ///<summary>Static Provider for RevitAPI</summary>
        public static DocProvider Provider => _provider ?? (_provider = new DocProvider());

        public string XMLDoc { get; private set; }

        #endregion

        #region Methods

        private void InitDoc()
        {
            try
            {
                XMLDoc = GetXmlData();
                if (!string.IsNullOrEmpty(XMLDoc))
                {
                    Doc = Deserialize(typeof(doc), XMLDoc) as doc;
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (Doc == null)
                {
                    Doc = new doc();
                }
            }
        }

        private string GetXmlData()
        {
            var dataPath = AssemblyPath + "\\doc\\RevitAPI.xml";
            if (File.Exists(dataPath))
            {
                return File.ReadAllText(dataPath);
            }
            //Assembly ass = Assembly.GetAssembly(typeof(doc));
            //var resourceName = ass.GetName().Name + ".g";
            //ResourceManager rm = new ResourceManager(resourceName, ass);
            //using (ResourceSet set = rm.GetResourceSet(CultureInfo.CurrentCulture, true, true))
            //{
            //    var dicSet = default(DictionaryEntry);
            //    //遍历资源集
            //    foreach (DictionaryEntry item in set)
            //    {
            //        if (item.Key.ToString() == "RevitAPI.xml")
            //        {
            //            dicSet = item;
            //        }
            //    }

            //    //使用资源流
            //    using (System.IO.UnmanagedMemoryStream UmMS = dicSet.Value as System.IO.UnmanagedMemoryStream)
            //    {
            //        if (UmMS.CanRead)
            //        {
            //            //反序列化
            //            using (StreamReader sr = new StreamReader(UmMS))
            //            {
            //                return sr.ReadToEnd();
            //            }
            //        }
            //    }
            //}
            return string.Empty;
        }

        public string AssemblyPath
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        public static object Deserialize(Type type, string xml)
        {
            using (StringReader sr = new StringReader(xml))
            {
                XmlSerializer xmldes = new XmlSerializer(type);
                return xmldes.Deserialize(sr);
            }
        }

        public member GetMemeber(string fullPath, memberType m)
        {
            var member = default(member);

            member = Doc.members.Where(p => p.memberType == memberType.T && p.name == $"{m.ToString()}:{fullPath}").FirstOrDefault();

            return member;
        }

        public member GetMemeber(string fullPath)
        {
            var member = default(member);

            member = Doc.members.Where(p => p.name.Contains(fullPath)).FirstOrDefault();

            return member;
        }

        public member GetMemeber(string memberNamespace, string typeName, string name, memberType m)
        {
            var member = default(member);
            switch (m)
            {
                case memberType.T:
                    member  = Doc.members.Where(p => p.memberType == memberType.T && p.name == $"T:{memberNamespace}.{typeName}").FirstOrDefault();
                    break;
                case memberType.M:
                    member = Doc.members.Where(p => p.memberType == memberType.T && p.name == $"M:{memberNamespace}.{typeName}.{name}").FirstOrDefault();
                    break;
                case memberType.P:
                    member = Doc.members.Where(p => p.memberType == memberType.T && p.name == $"P:{memberNamespace}.{typeName}.{name}").FirstOrDefault();
                    break;
            }
            return member;
        }

        public bool TryGetSummary(member member, out string summary, out string since)
        {
            summary = member?.summary;
            since = member?.remarks;
            return member != null;
        }

        #endregion

    }
}
