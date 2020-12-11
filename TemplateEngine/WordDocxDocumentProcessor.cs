using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using TemplateEngine.Commands;
using TemplateEngine.DataContext;
using Novacode;
/*using Xceed.Words.NET;

#if XCEED_LICENSED
using Xceed.Document.NET;
#endif*/

namespace TemplateEngine
{
    public class WordDocxDocumentProcessor : IWordDocxDocumentProcessor
    {
        public DocX Doc { get; }
        private readonly IReadOnlyDictionary<Type, Regex> _markupCommandTypes;
        private IMarkupExtractor _extractor;

        public WordDocxDocumentProcessor(DocX doc)
        {
            Doc = doc;
            var markupCommands = GetType().Assembly.GetTypes()
                .Where(t => t.IsSubclassOf(typeof(MarkupCommand)) && !t.IsAbstract).ToDictionary(t => t,
                    t => t.GetCustomAttribute<MarkupCommandPatternAttribute>());

            var markupCommandsAndPatterns = new Dictionary<Type, Regex>();
            foreach (var markupCommandType in markupCommands)
            {
                if (markupCommandType.Value == null)
                    throw new TemplateEngineException(
                        $"Необходимо указать атрибут {typeof(MarkupCommandPatternAttribute)} для класса {markupCommandType.Key}");

                if (string.IsNullOrEmpty(markupCommandType.Value.Pattern))
                    throw new TemplateEngineException(
                        $"Необходимо указать шаблон команды для класса {typeof(MarkupCommandPatternAttribute)}");

                var regex = new Regex(markupCommandType.Value.Pattern, markupCommandType.Value.RegexOptions);
                markupCommandsAndPatterns.Add(markupCommandType.Key, regex);
            }
            _markupCommandTypes = markupCommandsAndPatterns;
        }

        public void ApplyMarkup(Markup markupPosition, IMarkupExtractor extractor, IDataContext dataContext)
        {
            var commandText = markupPosition.MarkupMatch.Groups[1].Value;
            var markupInfo = _markupCommandTypes
                .Select(m => new {Type = m.Key, Match = m.Value.Match(commandText)})
                .FirstOrDefault(m => m.Match.Success);
            if (markupInfo == null)
                throw new TemplateEngineException($"Не распознана команда '{commandText}'");
            var markupCommand = (IMarkupCommand) Activator.CreateInstance(markupInfo.Type);
            markupCommand.Apply(markupPosition, markupInfo.Match, extractor, this, dataContext);
        }

        public Formatting GetFormatting(Paragraph p, int paragraphPosition)
        {
            var formattedText = p.MagicText?.Cast<FormattedText>().Reverse().First(t => t.index <= paragraphPosition);
            return formattedText?.formatting;
        }

        public TableCellPosition GetParagraphPositionInTable(XElement paragraphXml)
        {
            foreach (var table in Doc.Tables)
            {
                for (var rowIndex = 0; rowIndex < table.Rows.Count; rowIndex++)
                {
                    var row = table.Rows[rowIndex];
                    for (var cellIndex = 0; cellIndex < row.Cells.Count; cellIndex++)
                    {
                        var cell = row.Cells[cellIndex];
                        var paragraphIndex = -1;
                        foreach (var e in cell.Xml.Descendants())
                        {
                            if (e.Name.LocalName == "p")
                                ++paragraphIndex;
                            if (e == paragraphXml)
                            {
                                return new TableCellPosition
                                {
                                    Table = table,
                                    Row = row,
                                    RowIndex = rowIndex,
                                    Cell = cell,
                                    CellIndex = cellIndex,
                                    ParagraphIndex = paragraphIndex
                                };
                            }
                        }
                    }
                }
            }

            return null;
        }

        public IEnumerable<XElement> GetElementsBetween(XElement startXml, XElement endXml)
        {
            return startXml.Parent.Elements().SkipWhile(x => x != startXml).Skip(1).TakeWhile(x => x != endXml);
        }

        public void SetFormatting(Paragraph paragraph, Formatting formatting)
        {
            if (formatting.Bold == true)
                paragraph.Bold();
            if (formatting.CapsStyle != null)
                paragraph.CapsStyle(formatting.CapsStyle.Value);
            if (formatting.FontColor != null)
                paragraph.Color(formatting.FontColor.Value);
            if (formatting.FontFamily != null)
                paragraph.Font(formatting.FontFamily);
            if (formatting.Hidden == true)
                paragraph.Hide();
            if (formatting.Highlight != null)
                paragraph.Highlight(formatting.Highlight.Value);
            if (formatting.Italic == true)
                paragraph.Italic();
            if (formatting.Kerning != null)
                paragraph.Kerning(formatting.Kerning.Value);
            if (formatting.Misc != null)
                paragraph.Misc(formatting.Misc.Value);
            if (formatting.PercentageScale != null)
                paragraph.PercentageScale(formatting.PercentageScale.Value);
            if (formatting.Position != null)
                paragraph.Position(formatting.Position.Value);
            if (formatting.Script != null)
                paragraph.Script(formatting.Script.Value);
            if (formatting.Size != null)
                paragraph.FontSize(formatting.Size.Value);
            if (formatting.Spacing != null)
                paragraph.Spacing(formatting.Spacing.Value);
            if (formatting.StrikeThrough != null)
                paragraph.StrikeThrough(formatting.StrikeThrough.Value);
            if (formatting.UnderlineColor != null)
                paragraph.UnderlineColor(formatting.UnderlineColor.Value);
            if (formatting.UnderlineStyle != null)
                paragraph.UnderlineStyle(formatting.UnderlineStyle.Value);
        }
        
        public DocX GetDoc()
        {
            return Doc;
        }

        public int GetParagraphIndexByXml(XElement xml)
        {
            // OPTFIX var index = Doc.Paragraphs.ToList().FindIndex(p => p.Xml == xml);

            var index = -1;
            foreach (var x in Doc.Xml.Descendants())
            {
                if (x.Name.LocalName == "p")
                    ++index;

                if (x == xml)
                    return index;
            }
            return index;
        }

        public void RemoveContentBetween(Paragraph startParagraph, int startIndex, Paragraph endParagraph, int endIndex)
        {
            var contentElements = GetElementsBetween(startParagraph.Xml, endParagraph.Xml).ToList();
            foreach (var contentElement in contentElements)
                contentElement.Remove();
            if(startIndex < startParagraph.Text.Length)
                startParagraph.RemoveText(startIndex);
            if(endIndex > 0)
                endParagraph.RemoveText(0, endIndex);
        }

        public Markup DuplicateMarkupParagraph(Markup markup, bool autoInsert = true)
        {
            var xmlClone = new XElement(markup.ParagraphXml);
            var p = CreateParagraph(GetDoc(), xmlClone, markup);
            if(autoInsert)
                markup.Paragraph.InsertParagraphAfterSelf(p);
            var newMarkup = new Markup(markup, p);            
            return newMarkup;
        }

        private Paragraph CreateParagraph(DocX doc, XElement xElement, Markup markup)
        {
            var ctor = typeof(Paragraph).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null,
                new[] { typeof(DocX), typeof(XElement), typeof(int), typeof(ContainerType) }, null);
            var p = (Paragraph)ctor.Invoke(new object[] { doc, xElement, 0, markup.Paragraph.ParentContainer });
            p.PackagePart = doc.PackagePart;
            return p;
        }
    }
}