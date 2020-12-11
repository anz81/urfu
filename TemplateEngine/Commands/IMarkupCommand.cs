using System.Text.RegularExpressions;
using TemplateEngine.DataContext;

namespace TemplateEngine.Commands
{
    public interface IMarkupCommand
    {
        void Apply(Markup markup, Match commandMatch, IMarkupExtractor extractor, IWordDocxDocumentProcessor processor,
            IDataContext dataContext);
    }
}