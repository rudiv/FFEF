using System.IO;
using System.Threading.Tasks;

namespace FFEF.Infrastructure
{
    public class RazorViewRenderer
    {
        readonly RazorViewCompiler compiler;

        public RazorViewRenderer(RazorViewCompiler compiler)
        {
            this.compiler = compiler;
        }

        public async Task<string> RenderAsync(string viewPath)
        {
            var view = compiler.CompileView(viewPath);
            var ms = new MemoryStream();
            view.SetOutput(ms);
            await view.ExecuteAsync();

            if (view.Layout != null)
            {
                ms = new MemoryStream();
                var layout = (RazorLayout)compiler.CompileView(view.Layout);
                layout.Model = view;
                layout.SetOutput(ms);
                await layout.ExecuteAsync();
                layout.Finalise();
            } else
            {
                view.Finalise();
            }
            
            ms.Seek(0, SeekOrigin.Begin);

            return await (new StreamReader(ms)).ReadToEndAsync();
        }
    }
}
