using Microsoft.AspNetCore.Html;
using System.IO;
using System.Threading.Tasks;

namespace FFEF.Infrastructure
{
    public abstract class RazorView
    {
        protected TextWriter Output { get; set; }

        public string Layout { get; set; }

        public void SetOutput(Stream outputStream)
        {
            Output = new StreamWriter(outputStream);
        }
        public void Finalise()
        {
            Output.Flush();
        }

        protected void WriteLiteral(string literal)
        {
            if (!string.IsNullOrEmpty(literal))
            {
                Output.Write(literal);
            }
        }

        protected void Write(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                Output.Write(value);
            }
        }

        protected void Write(object value)
        {
            if (value == null || value == HtmlString.Empty)
            {
                return;
            }

            Output.Write(value.ToString());
        }

        public abstract Task ExecuteAsync();
    }
}
