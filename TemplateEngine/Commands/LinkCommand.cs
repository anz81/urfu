using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using TemplateEngine.DataContext;
//using Xceed.Words.NET;
using Novacode;

namespace TemplateEngine.Commands
{
    [MarkupCommandPattern(@"^link\s+(.+?)(?:\s*;\s*(.+)\s*)?$")]
    public class LinkCommand : MarkupCommand
    {
        public override void Apply(Markup openMarkup, Match commandMatch, IMarkupExtractor extractor, IWordDocxDocumentProcessor processor,
            IDataContext dataContext)
        {
            var urlPath = commandMatch.Groups[1].Value;
            var contentPath = commandMatch.Groups[2].Value;
            if (string.IsNullOrWhiteSpace(contentPath))
                contentPath = urlPath;
            var url = Convert.ToString(dataContext.Get(urlPath));
            var content = Convert.ToString(dataContext.Get(contentPath));
            var doc = processor.GetDoc();
            var uri = new Uri(url);
            var hyperlink = doc.AddHyperlink(content, uri);
            openMarkup.Paragraph.InsertHyperlink(hyperlink, openMarkup.Index + openMarkup.ReplaceMark.Length);
            openMarkup.RemoveMarkup();
        }
    }
}