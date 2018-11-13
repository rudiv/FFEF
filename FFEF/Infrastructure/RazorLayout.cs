using System.IO;
using System.Threading.Tasks;

namespace FFEF.Infrastructure
{
    public abstract class RazorLayout : RazorView<RazorView>
    {
        public async Task<string> RenderBody()
        {
            var ms = new MemoryStream();
            Model.SetOutput(ms);
            await Model.ExecuteAsync();
            Model.Finalise();
            ms.Seek(0, SeekOrigin.Begin);

            var body = await (new StreamReader(ms)).ReadToEndAsync();

            return body;
        }
    }
}