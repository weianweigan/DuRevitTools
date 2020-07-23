using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using DuRevitTools.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace DuRevitTools.APIAnalyzer.RevitRefAnalyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class RevitRefAnalyzer: DiagnosticAnalyzer
    {
        #region Fields

        // You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
        // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Localizing%20Analyzers.md for more on localization
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.RevitRefTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.RevitRefMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.RevitRefDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Usage";

        private static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticIDs.RevitRefAnalyzer, Title, MessageFormat, Category, DiagnosticSeverity.Info, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        #endregion

        #region Property
        public static DocProvider DocProvider => DocProvider.Provider;

        #endregion

        public override void Initialize(AnalysisContext context)
        {
            // TODO: Consider registering other actions that act on syntax instead of or in addition to symbols
            // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Analyzer%20Actions%20Semantics.md for more information
            //context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.NamedType);

            context.RegisterSyntaxNodeAction(AnalyzerSyntaxNode, SyntaxKind.PropertyDeclaration, SyntaxKind.FieldDeclaration, SyntaxKind.IdentifierName,SyntaxKind.IdentifierName);
        }

        #region Methods

        ///<summary>Analyzer node which is a revit type or not,if it is ,then report for user to rereference RevitAPI.dll</summary>
        private void AnalyzerSyntaxNode(SyntaxNodeAnalysisContext context)
        {
            TypeSyntax type = null;

            //判断当前项目是否运营Revit.dll
            var revitAPIRef = context.Compilation.References.Where(p => Path.GetFileNameWithoutExtension(p.Display) == "RevitAPI").FirstOrDefault();

            if (revitAPIRef != null)
            {
                return;
            }

            //获取Type类型的Symbol
            var node = context.Node;
            switch (context.Node.Kind())
            {
                case SyntaxKind.PropertyDeclaration:
                    type = ((PropertyDeclarationSyntax)node).Type;
                    break;
                case SyntaxKind.FieldDeclaration:
                    type = ((FieldDeclarationSyntax)node).Declaration.Type;
                    break;
                case SyntaxKind.IdentifierName:
                    type = ((IdentifierNameSyntax)node);
                    break;
                default:
                    return;
            }

            if (type == null)
            {
                return;
            }

            //获取Symbol对象
            var symbol = context.SemanticModel.GetSymbolInfo(type).Symbol;

            //找到Symbol的话 Return
            if (symbol != null)
            {
                return;
            }


            var namespaces = context.Node.SyntaxTree.GetRoot().ChildNodes().Where(p => p.Kind() == SyntaxKind.UsingDirective);

            if (SymbolHelper.IsRevitAPITypeWithNameSpace(type.ToString(), namespaces,out var m))
            {
                var diagnostic = Diagnostic.Create(Rule, node.GetLocation(), m.name);
                context.ReportDiagnostic(diagnostic);
            }
        }

        #endregion
    }
}
