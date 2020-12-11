using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using Microsoft.CSharp;
using TemplateEngine.DataContext;

namespace TemplateEngine.Commands
{
    [MarkupCommandPattern(@"^(?<commandName>if)\s+(.+)$", RegexOptions.IgnoreCase)]
    public class IfCommand : MarkupCommand
    {
        public override void Apply(Markup openMarkup, Match commandMatch, IMarkupExtractor extractor,
            IWordDocxDocumentProcessor processor, IDataContext dataContext)
        {
            var expression = commandMatch.Groups[1].Value;

            var markups = extractor.ExtractNestedMarkups(commandMatch.Groups["commandName"].Value, out var closeMarkup).ToList();
            
            var condition = (bool) dataContext.Get(expression);

            if (!condition)
            {
                processor.RemoveContentBetween(openMarkup.Paragraph, openMarkup.Index + openMarkup.ReplaceMark.Length,
                    closeMarkup.Paragraph, closeMarkup.Index);
                closeMarkup.RemoveMarkup();
                openMarkup.RemoveMarkup();
            }
            else
            {
                openMarkup.RemoveMarkup();
                closeMarkup.RemoveMarkup();
                var memoryExtractor = new MemoryMarkupExtractor(markups);
                foreach (var markup in memoryExtractor.ExtractMarkups())
                    processor.ApplyMarkup(markup, memoryExtractor, dataContext);                
            }            
        }
    }       
}