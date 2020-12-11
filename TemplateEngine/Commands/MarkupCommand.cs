using System.Text.RegularExpressions;
using TemplateEngine.DataContext;

namespace TemplateEngine.Commands
{
    public abstract class MarkupCommand : IMarkupCommand
    {
        public abstract void Apply(Markup openMarkup, Match commandMatch, IMarkupExtractor extractor,
            IWordDocxDocumentProcessor processor, IDataContext dataContext);
    }
}