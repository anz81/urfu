using System.Collections.Generic;
using System.Linq;

namespace TemplateEngine
{
    public class MemoryMarkupExtractor : IMarkupExtractor
    {
        private readonly List<Markup> _markups;

        public MemoryMarkupExtractor(IEnumerable<Markup> markups)
        {
            _markups = markups.ToList();
        }

        public IEnumerable<Markup> ExtractMarkups()
        {
            var first = _markups.FirstOrDefault();
            while (first != null)
            {
                _markups.Remove(first);
                yield return first;
                first = _markups.FirstOrDefault();
            }
        }

        public IEnumerable<Markup> ExtractNestedMarkups(string commandName, out Markup closeMarkup)
        {
            Markup closeMarkup2 = null;
            var openCounter = 1;
            var markups = _markups.TakeWhile(m =>
            {
                closeMarkup2 = m;
                var isOpenMarkup = m.CommandText.Split(' ')[0] == commandName;
                if (isOpenMarkup)
                {
                    ++openCounter;
                    return true;
                }

                var isCloseMarkup = m.CommandText == $"/{commandName}";
                if (isCloseMarkup)
                {
                    --openCounter;
                    if (openCounter == 0)
                        return false;
                }

                return true;
            }).ToList();

            closeMarkup = closeMarkup2;

            foreach (var markup in markups)
                _markups.Remove(markup);
            _markups.Remove(closeMarkup);

            return markups;
        }
    }
}