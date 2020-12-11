using System.Collections.Generic;

namespace TemplateEngine
{
    public interface IMarkupExtractor
    {
        IEnumerable<Markup> ExtractMarkups();

        IEnumerable<Markup> ExtractNestedMarkups(string commandName, out Markup closeMarkup);
    }
}