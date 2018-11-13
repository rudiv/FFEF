using Microsoft.AspNetCore.Razor.Hosting;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.AspNetCore.Razor.Language.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace FFEF.Infrastructure
{
    public class RazorViewCompiler
    {
        protected RazorProjectFileSystem projectFileSystem;
        protected RazorProjectEngine projectEngine;

        public RazorViewCompiler(string basePath)
        {
            projectFileSystem = RazorProjectFileSystem.Create(basePath);

            projectEngine = RazorProjectEngine.Create(RazorConfiguration.Default, projectFileSystem, cfg =>
            {
                cfg.SetBaseType("FFEF.Infrastructure.RazorView");
                cfg.SetNamespace("CompiledView");

                InheritsDirective.Register(cfg);
                ModelDirective.Register(cfg);
            });
        }

        public RazorView CompileView(string viewPath)
        {
            var item = projectFileSystem.GetItem(viewPath);

            var tree = GetSyntaxTreeForItem(item);
            var asm = CompileTreeToAnonymousAssembly(tree);

            return (RazorView)Activator.CreateInstance(asm.GetType("CompiledView.Template"));
        }

        protected SyntaxTree GetSyntaxTreeForItem(RazorProjectItem item)
        {
            var codeDocument = projectEngine.Process(item);
            var csharp = codeDocument.GetCSharpDocument();
            //Console.WriteLine(csharp.GeneratedCode);
            return CSharpSyntaxTree.ParseText(csharp.GeneratedCode);
        }

        protected Assembly CompileTreeToAnonymousAssembly(SyntaxTree tree)
        {
            string assemblyName = Path.GetRandomFileName();
            MetadataReference[] references = new MetadataReference[]
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(RazorView).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location),
                MetadataReference.CreateFromFile(Path.Combine(Path.GetDirectoryName(typeof(object).Assembly.Location), "netstandard.dll")),
                MetadataReference.CreateFromFile(typeof(RazorCompiledItemAttribute).Assembly.Location),
            };

            CSharpCompilation compilation = CSharpCompilation.Create(
                assemblyName,
                syntaxTrees: new[] { tree },
                references: references,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            var asm = default(Assembly);
            using (var ms = new MemoryStream())
            {
                var result = compilation.Emit(ms);

                if (!result.Success)
                {
                    IEnumerable<Diagnostic> failures = result.Diagnostics.Where(diagnostic =>
                        diagnostic.IsWarningAsError ||
                        diagnostic.Severity == DiagnosticSeverity.Error);

                    foreach (Diagnostic diagnostic in failures)
                    {
                        Console.Error.WriteLine("{0}: {1}", diagnostic.Id, diagnostic.GetMessage());
                    }
                }
                else
                {
                    ms.Seek(0, SeekOrigin.Begin);
                    asm = Assembly.Load(ms.ToArray());
                }
            }

            return asm;
        }
    }
}
