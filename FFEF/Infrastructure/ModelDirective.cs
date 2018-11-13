using Microsoft.AspNetCore.Razor.Language;
using System;

namespace FFEF.Infrastructure
{
    /// <summary>
    /// Support for @model shorthand for default @inherits
    /// </summary>
    public static class ModelDirective
    {
        public static readonly string ModelKeyword = "model";

        public static readonly DirectiveDescriptor Directive = DirectiveDescriptor.CreateDirective(
            ModelKeyword,
            DirectiveKind.SingleLine,
            builder =>
            {
                builder.AddTypeToken("TypeName", "Type of Model that the Page should use.");
                builder.Usage = DirectiveUsage.FileScopedSinglyOccurring;
                builder.Description = "Define the Model Type that the Page should use.";
            });

        public static void Register(RazorProjectEngineBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.AddDirective(Directive);
            builder.Features.Add(new ModelDirectiveFeature());
        }
    }
}
