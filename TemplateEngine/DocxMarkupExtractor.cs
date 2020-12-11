using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Novacode;
/*using Xceed.Words.NET;

#if XCEED_LICENSED
using Xceed.Document.NET;
#endif*/

namespace TemplateEngine
{
    public class DocxMarkupExtractor : IMarkupExtractor
    {
        private readonly bool _debugMode;
        public DocX Doc { get; }
        public IWordDocxDocumentProcessor Processor { get; }

        public DocxMarkupExtractor(DocX doc, IWordDocxDocumentProcessor processor, bool debugMode)
        {
            _debugMode = debugMode;
            Doc = doc;
            Processor = processor;
        }

        public IEnumerable<Markup> ExtractNestedMarkups(string commandName, out Markup closeMarkup)
        {
            Markup closeMarkup2 = null;
            var openCounter = 1;
            var markups = ExtractMarkups().TakeWhile(m =>
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
            return markups;
        }

        public IEnumerable<Markup> ExtractMarkups()
        {
            var markupPattern = "<<(.*?)>>";
            var regex = new Regex(markupPattern, RegexOptions.Compiled);
            var counter = 0;

//            var footerParagraphs = (Doc.Footers.First?.Paragraphs ?? Enumerable.Empty<Paragraph>()).Concat(Doc.Footers.Even?.Paragraphs ?? Enumerable.Empty<Paragraph>()).Concat(Doc.Footers.Odd?.Paragraphs ?? Enumerable.Empty<Paragraph>());
//            var headerParagraphs = (Doc.Headers.First?.Paragraphs ?? Enumerable.Empty<Paragraph>()).Concat(Doc.Headers.Even?.Paragraphs ?? Enumerable.Empty<Paragraph>()).Concat(Doc.Headers.Odd?.Paragraphs ?? Enumerable.Empty<Paragraph>());
            var bodyParagraphs = Doc.Paragraphs;
            var paragraphs = bodyParagraphs;// headerParagraphs.Concat(footerParagraphs).Concat(bodyParagraphs).ToList();

            var dic = new Dictionary<string, Markup>();

            foreach (Novacode.Paragraph p in paragraphs)
            {
                var m = regex.Match(p.Text);
                while (m.Success)
                {
                    ++counter;
                    //var clone = new XElement(p.Xml);
                    Formatting formatting = null;
                    try
                    {
                        //formatting = Formatting.Parse(clone);
                        formatting = Processor.GetFormatting(p, m.Index + 2);
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine($"Parse formatting error:{Environment.NewLine}{ex}");
                    }

                    if (_debugMode)
                    {
                        if (formatting != null)
                            formatting.FontColor = Color.CornflowerBlue;
                        else
                        {
                            formatting = new Formatting
                            {
                                FontColor = Color.CornflowerBlue
                            };
                        }
                    }

                    var replaceMark = GetUniqueMark();                    
                    p.InsertText(m.Index, replaceMark, false, formatting);
                    p.RemoveText(m.Index+replaceMark.Length, m.Value.Length);
                    
                    var markup = new Markup
                    {
                        MarkupMatch = m,
                        MarkupPattern = markupPattern,
                        Index = m.Index,
                        Paragraph = p,
                        MarkupText = m.Value,
                        Formatting = formatting,
                        CommandText = m.Groups[1].Value,                        
                        ReplaceMark = replaceMark
                    };

                    dic.Add(replaceMark, markup);

                    yield return markup;                    

                    m = regex.Match(p.Text);                    
                }
            }
        }

        private string GetUniqueMark()
        {
            return Guid.NewGuid().ToString();
        }
    }
}