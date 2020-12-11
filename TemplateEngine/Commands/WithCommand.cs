using System.Linq;
using System.Text.RegularExpressions;
using TemplateEngine.DataContext;

namespace TemplateEngine.Commands
{
    [MarkupCommandPattern(@"^(?<commandName>with)\s+(.+)$", RegexOptions.IgnoreCase)]
    public class WithCommand : MarkupCommand
    {
        public override void Apply(Markup openMarkup, Match commandMatch, IMarkupExtractor extractor, IWordDocxDocumentProcessor processor, IDataContext dataContext)
        {
            var expression = commandMatch.Groups[1].Value;
            var obj = dataContext.Get(expression);
            
            var markups = extractor.ExtractNestedMarkups(commandMatch.Groups["commandName"].Value, out var closeMarkup).ToList();
            openMarkup.RemoveMarkup();
            closeMarkup.RemoveMarkup();

            using (dataContext.Change(new ScopeChangeDescriptor(obj)))
            {
                var memoryExtractor = new MemoryMarkupExtractor(markups);
                foreach (var markup in memoryExtractor.ExtractMarkups())
                {
                    processor.ApplyMarkup(markup, memoryExtractor, dataContext);
                }
            }
        }
    }
}