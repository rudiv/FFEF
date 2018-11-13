using FFEF.Infrastructure;
using Microsoft.AspNetCore.Razor.Hosting;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.AspNetCore.Razor.Language.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FFEF
{
    public static class Program
    {
        static RazorViewRenderer renderer;
        static string outPath;
        static async Task Main(string[] args)
        {
            /*
             * Creating a static site is no fun
             * It's even less fun when the recommended path in 2018 is to add 100s of MBs of node_modules
             * 
             * but Razor is fun, so let's use that
             */

            if (args.Length == 0)
            {
                renderer = new RazorViewRenderer(new RazorViewCompiler("test"));
                Console.WriteLine(await renderer.RenderAsync("Views/Test.cshtml"));

            } else
            {
                renderer = new RazorViewRenderer(new RazorViewCompiler(args[0]));
                outPath = args[1];
                var fsWatcher = new FileSystemWatcher(args[0]);
                fsWatcher.Changed += FsWatcher_Changed;
                fsWatcher.EnableRaisingEvents = true;
                Console.WriteLine("Watching...");
            }
            Console.ReadLine();
        }

        private static async void FsWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            try
            {
                var html = await renderer.RenderAsync(e.FullPath);
                var fileOut = Path.Combine(outPath, Path.GetFileNameWithoutExtension(e.FullPath) + ".htm");
                File.WriteAllText(fileOut, html);
                Console.WriteLine("Compiled " + e.FullPath + " to " + fileOut);
            } catch(Exception ex)
            {
                Console.Error.WriteLine($"Couldn't compile {e.FullPath}... {ex.Message}");
            }
        }
    }
}
