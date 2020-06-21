using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using System.Threading.Tasks;
using DuRevitTools.Model;

namespace DuRevitTools.DocGenerator
{
    public class helpHtmlPraser
    {
        private readonly string _htmlFilePath;

        public helpHtmlPraser(string htmlFilePath)
        {
            _htmlFilePath = htmlFilePath;
        }

        public string Name { get; set; }

        public member Member { get; set; }

        [XmlIgnore]
        public bool IsMember { get; internal set; } = true;

        public helpHtmlPraser Init()
        {
            return InitAsync().GetAwaiter().GetResult();
        }

        public async Task<helpHtmlPraser> InitAsync()
        {
            try
            {
                //读取字符串
                string text = System.IO.File.ReadAllText(_htmlFilePath);

                var context = BrowsingContext.New(Configuration.Default);

                var doc = await context.OpenAsync(req => req.Content(text));

                Name = GetHeadContent(doc);

                Member = GetMember(Name);

                Member.summary = GetSummary(doc);

                Member.remarks = GetRemarks(doc);

                if (Member.memberType == memberType.M)
                {
                    //获取返回值
                    Member.returns = GetReturns(doc);

                    //获取参数
                    var @params = GetParams(doc);

                    foreach (var item in @params)
                    {
                        Member.@params.Add(item);
                    }
                }
                else if (Member.memberType == memberType.P)
                {

                }
                //doc.Head.GetElementsByClassName("meta")
                //读取方法名
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                IsMember = false;
            }

            return this;
        }

        private List<Tuple<string,string>> GetExceptionList(IDocument doc)
        {

            return null;
        }

        private string GetRemarks(IDocument doc)
        {
            return doc.Body.GetElementsByTagName("div").Where(p => p.Id == "remarksSection").FirstOrDefault()?.TextContent;
        }

        private string GetSummary(IDocument doc)
        {
            return doc.Body.GetElementsByClassName("summary").FirstOrDefault()?.TextContent;
        }

        ///<summary>获取参数</summary>
        private IEnumerable<param> GetParams(IDocument doc)
        {
            var docparams = doc.Body.GetElementsByTagName("div").Where(p => p.Id == "parameters").FirstOrDefault()?.GetElementsByTagName("dl");

            if (docparams == null)
            {
                yield break;
            }

            foreach (var d1Node in docparams)
            {
                param par = new param();
                par.name = d1Node.GetElementsByTagName("span").FirstOrDefault().TextContent;

                var ddele = d1Node.GetElementsByTagName("dd").FirstOrDefault();

                par.value = ParseParamValue(ddele.TextContent);//.Replace("&lt;", "<").Replace("/&gt;", "/>");

                yield return par;
            }
        }

        private string ParseParamValue(string value)
        {
            value = value.Replace("..::..", ".");
            try
            {
                if (value.StartsWith("Type:"))
                {
                    var path = value.Split(':', ' ')[2];
                    string seeAlso = InsertSeeAlso(memberType.T, path);
                    return value.Replace($"Type: {path}", seeAlso);
                };
            }
            catch(Exception ex)
            {

            }
            return value;
        }

        ///<summary>获取返回值</summary>
        private string GetReturns(IDocument doc)
        {
            var eles = doc.Body.GetElementsByTagName("div").Where(p => p.Id == "syntaxSection").FirstOrDefault();

            return eles?.InnerHtml.Split('>').LastOrDefault();
        }

        ///<summary>获取成员名字</summary>
        ///<param name="doc">dsfd <see cref="string"/>
        ///dsfd</param>
        private static string GetHeadContent(IDocument doc,string headName = "Microsoft.Help.Id")
        {
            foreach (var item in doc.Head.ChildNodes)
            {
                if (item is IHtmlMetaElement)
                {
                    var htmlele = item as IHtmlMetaElement;
                    if (htmlele.Name == headName)
                    {
                        return htmlele.Content;
                    }
                }
            }
            return string.Empty;
        }

        public string InsertSeeAlso(memberType memberType,string path)
        {
            return path;
            //return $"<see cref = \"{memberType.ToString()}:{path}\" />";
        }

        ///<summary>后去<see cref="member"/></summary>
        private member GetMember(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new Exception($"{_htmlFilePath} Microsoft.Help.Id Error");
            }

            member member = default;

            //名字
            switch (name[0])
            {
                case 'M':
                    member = new member(memberType.M);
                    break;
                case 'T':
                    member = new member(memberType.T);
                    break;
                case 'P':
                    member = new member(memberType.P);
                    break;
                default:
                    throw new Exception($"{_htmlFilePath}--- Error Name {name}");
            }

            member.name = name;

            return member;
        }
    }
}
