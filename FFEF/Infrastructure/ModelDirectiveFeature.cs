using Microsoft.AspNetCore.Razor.Language;
using Microsoft.AspNetCore.Razor.Language.Intermediate;
using System.Linq;

namespace FFEF.Infrastructure
{
    /// <summary>
    /// Support for @model
    /// </summary>
    public class ModelDirectiveFeature : IntermediateNodePassBase, IRazorDirectiveClassifierPass
    {
        protected override void ExecuteCore(RazorCodeDocument codeDocument, DocumentIntermediateNode documentNode)
        {
            var @class = documentNode.FindPrimaryClass();
            if (@class == null)
            {
                return;
            }

            foreach (var inherits in documentNode.FindDirectiveReferences(ModelDirective.Directive))
            {
                var token = ((DirectiveIntermediateNode)inherits.Node).Tokens.FirstOrDefault();
                if (token != null)
                {
                    @class.BaseType = $"FFEF.Infrastructure.RazorView<{token.Content}>";
                    break;
                }
            }
        }
    }
}
