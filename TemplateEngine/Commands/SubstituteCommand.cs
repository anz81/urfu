using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
//using System.Windows.Documents;
using System.Xml.Linq;
using TemplateEngine.DataContext;

namespace TemplateEngine.Commands
{
    [MarkupCommandPattern(@"^(?:\[)(.*)(?:\])$")]
    public class SubstituteCommand : MarkupCommand
    {
        private Regex _linkRegex = new Regex(@"(https?|ftp)://[^\s/$.?#].[^\s]*", RegexOptions.Compiled);

        public override void Apply(Markup markup, Match commandMatch, IMarkupExtractor extractor,
            IWordDocxDocumentProcessor processor, IDataContext dataContext)
        {
            var format = commandMatch.Groups[2].Value;
            var value = dataContext.Get(commandMatch.Groups[1].Value);

            var checkLinks = true;

            if (value is decimal
                || value is double
                || value is int
                || value is short
                || value is float)
            {
                if (string.IsNullOrEmpty(format))
                    format = "G29";
                checkLinks = false;
            }

            var formattedValue = string.Format("{0:" + format + "}", value);

            if (value is Image image)
            {
                var stream = new MemoryStream();
                image.Save(stream, image.RawFormat);
                stream.Seek(0, SeekOrigin.Begin);
                var img = processor.GetDoc().AddImage(stream);
                markup.InsertImage(img);
            }
            else
            {
                if (!formattedValue.Contains('\n'))
                {
                    markup.InsertText(formattedValue);
                }
                else
                {
                    var texts = formattedValue.Split(new[] {'\n'}, StringSplitOptions.None);
                    InsertMultiText(texts, markup, processor, extractor, dataContext);
                }
            }

            // TODO доделать бы
            /*if (checkLinks)
            {
                var doc = processor.GetDoc();
                var matches = _linkRegex.Matches(formattedValue).Cast<Match>().Reverse();
                foreach (var m in matches)
                {
                    if (m.Success)
                    {
                        var hyperlink = doc.AddHyperlink(m.Value, new Uri(m.Value));
                        markup.Paragraph.InsertHyperlink(hyperlink, m.Index);
                    }
                }
            }*/
        }

        private void InsertMultiText(string[] texts, Markup markup, IWordDocxDocumentProcessor processor, IMarkupExtractor extractor, IDataContext dataContext)
        {
            XElement runToSplit = null;
            int runToSplitIndex = -1;
            int replaceMarkInnerIndex = -1;
            for (var i = 0;
                i < markup.ParagraphXml.Descendants().Where(d => d.Name.LocalName == "r").ToList().Count;
                i++)
            {
                var e = markup.ParagraphXml.Descendants().Where(d => d.Name.LocalName == "r").ToList()[i];
                var index = e.Value.IndexOf(markup.ReplaceMark);
                if (index >= 0)
                {
                    runToSplit = e;
                    replaceMarkInnerIndex = index;
                    runToSplitIndex = i;
                    break;
                }
            }

            var allMarkups = new List<Markup>{markup};
            for (var index = 1; index < texts.Length; index++)
                allMarkups.Add(processor.DuplicateMarkupParagraph(allMarkups.Last(), true));

            for (var index = 0; index < texts.Length; index++)
            {
                //var p = texts[index];
                var pMarkup = allMarkups[index];
                if (index == 0)
                {
                    var first = pMarkup.ParagraphXml;
                    first.Elements().Where(e => e.Name.LocalName == "r").Skip(runToSplitIndex+1).Remove();
                }

                if (index != 0 && index != texts.Length - 1)
                {
                    var toRemove = new List<XElement>();
                    toRemove.AddRange(pMarkup.ParagraphXml.Elements().Where(e => e.Name.LocalName == "r").Take(runToSplitIndex));
                    toRemove.AddRange(pMarkup.ParagraphXml.Elements().Where(e => e.Name.LocalName == "r").Skip(runToSplitIndex+1));
                    toRemove.ForEach(r=>r.Remove());
                }

                if (index == texts.Length - 1)
                {
                    var last = pMarkup.ParagraphXml;
                    last.Elements().Where(e => e.Name.LocalName == "r").Take(runToSplitIndex).Remove();
                }
                //var newParagraphXml = new XElement(markup.ParagraphXml);

                //markup.ParagraphXml.AddAfterSelf(newParagraphXml);
                //newParagraphs.Add(newParagraphXml);
                //var newMarkup = new Markup(markup, markup.Paragraph.InsertParagraphAfterSelf(markup.MarkupText));
                //markups.Add(newMarkup);
            }

            for (var index = 0; index < texts.Length; index++)
            {
                var p = texts[index];
                var pMarkup = allMarkups[index];
                //if (index > 0)
                    //pMarkup.Paragraph.InsertParagraphAfterSelf(pMarkup.Paragraph);
                //dataContext.Change(new ScopeChangeDescriptor(dataContext.))
                using (var lifetime = dataContext.Change(new ScopeChangeDescriptor(p)
                {
                    Vars = { { pMarkup.CommandText.TrimStart('[').TrimEnd(']'), p} }
                }))
                {
                    processor.ApplyMarkup(pMarkup, extractor, dataContext);
                }
            }
        }
    }
}