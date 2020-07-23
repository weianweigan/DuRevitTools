using DuRevitTools.Model;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace DuRevitTools.APIAnalyzer
{
    public static class SymbolHelper
    {
        public static DocProvider DocProvider => DocProvider.Provider;

        private const string RevitNamespace = "Autodesk.Revit";

        ///<summary>Judge whether it is a Revit API</summary>
        public static bool IsRevitAPIType(ISymbol symbol, out member member)
        {
            member = default(member);

            var nameSpace = symbol.ContainingNamespace.ToString();
            var name = symbol.Name;
            var fullName = symbol.ToString();

#if DEBUG

            Debug.Print(nameSpace);

#endif
            if (!nameSpace.Contains(RevitNamespace))
            {
                return false;
            }

            member = DocProvider.GetMemeber(fullName, memberType.T);

            if (member == null)
            {
                return false;
            }

            return true;
        }


        ///<summary>仅仅通过类型名判断是否是Revit的类型</summary>
        public static bool IsRevitAPITypeWithNameSpace(string name, IEnumerable<SyntaxNode> namespaces, out member member)
        {
            var names = namespaces.Where(p => p.ToString().Contains("Autodesk.Revit")).Select(p => p.ToString());

            var spaces = names.Select(p =>
            {
                var pre = p.Replace(";","").Split(' ').LastOrDefault();
                return $"{pre}.{name}";
            }
                );

            member = DocProvider.Doc.members.Where(m => 
            spaces.Where(p => p == m.name.Split(':').LastOrDefault()).Count() >= 1
            ).FirstOrDefault();

            return member != null;
        }
    }
}
