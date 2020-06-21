using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using DuRevitTools.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace DuRevitTools.APIAnalyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class XMLDocAnalyzer : DiagnosticAnalyzer
    {
        #region Fields

        private const string RevitNamespace = "Autodesk.Revit";

        // You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
        // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/Localizing%20Analyzers.md for more on localization
        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.XMLDocTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.XMLDocMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.XMLDocDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Usage";

        private static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticIDs.XMLDocAnalyzer, Title, MessageFormat, Category, DiagnosticSeverity.Info, isEnabledByDefault: true, description: Description);

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

            context.RegisterSyntaxNodeAction(AnalyzerSyntaxNode, SyntaxKind.PropertyDeclaration, SyntaxKind.FieldDeclaration, SyntaxKind.PropertyKeyword, SyntaxKind.FieldKeyword);
        }

        #region Methods

        ///<summary>Analyzer node which is a revit type or not,if it is ,then report for user to rereference RevitAPI.dll</summary>
        private void AnalyzerSyntaxNode(SyntaxNodeAnalysisContext context)
        {
            TypeSyntax type = null;

            var node = context.Node;
            switch (context.Node.Kind())
            {
                case SyntaxKind.PropertyDeclaration:
                    type = ((PropertyDeclarationSyntax)node).Type;
                    break;
                case SyntaxKind.FieldDeclaration:
                    type = ((FieldDeclarationSyntax)node).Declaration.Type;
                    break;
                default:
                    return;
            }

            if (type == null)
            {
                return;
            }

            Debug.Print(node.ToFullString());

            var symbol = context.SemanticModel.GetSymbolInfo(type).Symbol;

            if (symbol == null)
            {
                return;
            }

            if (IsRevitAPIType(symbol,out _))
            {
                // Find Meta Data(RevitAPI.dll),judge whether it has a xml doc file;
                var revitApiRef = context.Compilation.References.Where(p => Path.GetFileNameWithoutExtension(p.Display) == "RevitAPI").FirstOrDefault();

                if (revitApiRef == null || XMLDocCodeFixProvider.IsXmlDocExist(revitApiRef.Display))
                {
                    return;
                }

                var diagnostic = Diagnostic.Create(Rule, node.GetLocation());
                context.ReportDiagnostic(diagnostic);
            }
        }

        ///<summary>Judge whether it is a Revit API</summary>
        private bool IsRevitAPIType(ISymbol symbol,out member member)
        {
            member = default(member);

            var nameSpace = symbol.ContainingNamespace.ToString();
            var name = symbol.Name;
            var fullName = symbol.ToString();

            Debug.Print(nameSpace);

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

        #endregion
    }
}
