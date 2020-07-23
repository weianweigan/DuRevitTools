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
using System.Globalization;
using System.Text;
using DuRevitTools.Service;
using System;
using DuRevitTools.Model;

namespace DuRevitTools.APIAnalyzer
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(RevitRefCodeFixProvider)), Shared]
    public class RevitRefCodeFixProvider: CodeFixProvider
    {
        private const string title = "Add reference of RevitAPI.dll for projcet";

        public sealed override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIDs.RevitRefAnalyzer); }
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
                    createChangedSolution: c => AddRevitReference(context.Document, declaration, c),
                    equivalenceKey: title),
                diagnostic);
        }

        private async Task<Solution> AddRevitReference(Document document, TypeDeclarationSyntax typeDecl, CancellationToken cancellationToken)
        {
            
            var originalSolution = document.Project.Solution;

            var versions = RevitPathProvider.Default.GetInstallVersion();

            var version = versions.FirstOrDefault();

            var revitApiPath = version.GetRevitAPI();

            var desPath = CopyToDesPath(revitApiPath,document.Project.Solution.FilePath ?? document.Project.FilePath,version.Version);

            var newproject = document.Project.AddMetadataReference(MetadataReference.CreateFromFile(desPath.Item1, default(MetadataReferenceProperties), XmlDocumentationProvider.CreateFromFile(desPath.Item2))) ;
            
            return newproject.Solution;
        }

        private Tuple<string,string> CopyToDesPath(string revitApiPath,string desFilePath,int version,bool withDoc = true)
        {
            var dir = Path.GetDirectoryName(desFilePath);
            var packageDir = Path.Combine(dir, "packages", $"Revit{version}");
            var name = Path.GetFileNameWithoutExtension(revitApiPath);

            var revitDesPath = Path.Combine(packageDir,$"{name}.dll");
            var revitDocDesPath = Path.Combine(packageDir, $"{name}.xml");


            if (!Directory.Exists(packageDir))
            {
                Directory.CreateDirectory(packageDir);
            }

            if (!File.Exists(revitDesPath))
            {
                File.Copy(revitApiPath, revitDesPath);
            }
            if (!File.Exists(revitDocDesPath))
            {
                File.WriteAllText(revitDocDesPath, SymbolHelper.DocProvider.XMLDoc);
            }

            return Tuple.Create(revitDesPath,revitDocDesPath);
        }
    }
}
