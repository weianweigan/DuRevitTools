using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.IO;
using System.Text;

namespace DuRevitTools.APIAnalyzer
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(XMLDocCodeFixProvider)), Shared]
    public class XMLDocCodeFixProvider: CodeFixProvider
    {
        private const string title = "Enable document for RevitAPI.dll (ReStart Visual Studio)";

        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIDs.XMLDocAnalyzer); }
        }

        public sealed override FixAllProvider GetFixAllProvider()
        {
            // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/FixAllProvider.md for more information on Fix All Providers
            return WellKnownFixAllProviders.BatchFixer;
        }

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            // TODO: Replace the following code with your own analysis, generating a CodeAction for each fix to suggest
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            // Find the type declaration identified by the diagnostic.
            var declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<TypeDeclarationSyntax>().First();

            // Register a code action that will invoke the fix.
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: title,
                    createChangedSolution: c => AddXmlDocAsync(context.Document, declaration, c),
                    equivalenceKey: title),
                diagnostic);
        }

        private async Task<Solution> AddXmlDocAsync(Document document, TypeDeclarationSyntax typeDecl, CancellationToken cancellationToken)
        {
            var revitAPIRef = document.Project.MetadataReferences.Where(p => Path.GetFileNameWithoutExtension(p.Display) == "RevitAPI").FirstOrDefault();

            var originalSolution = document.Project.Solution;

            if (revitAPIRef == null)
            {
                return originalSolution;
            }

            //生成XML文件
            XmlDocHelper.AddXMLFileToRef(revitAPIRef.Display);

            var newproject = document.Project
                .RemoveMetadataReference(revitAPIRef)
                .AddMetadataReference(MetadataReference
                .CreateFromFile(revitAPIRef.Display, default(MetadataReferenceProperties), 
                XmlDocumentationProvider.CreateFromBytes(Encoding.ASCII.GetBytes(SymbolHelper.DocProvider.XMLDoc))));

            return newproject.Solution;
        }
    }
}
