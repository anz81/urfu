using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using TemplateEngine.DataContext;
//using Xceed.Words.NET;
using Novacode;

#if XCEED_LICENSED
using Xceed.Document.NET;
#endif

namespace TemplateEngine.Commands
{
    [MarkupCommandPattern(@"^(?<commandName>foreach)\s+(?:([a-z0-9_]+)\s+in\s+)?(.+)$", RegexOptions.IgnoreCase)]
    public class ForeachCommand : MarkupCommand
    {
        public override void Apply(Markup openMarkup, Match commandMatch, IMarkupExtractor extractor,
            IWordDocxDocumentProcessor processor, IDataContext dataContext)
        {
            var markups = extractor.ExtractNestedMarkups(commandMatch.Groups["commandName"].Value, out var closeMarkup)
                .ToList();

            var startXml = openMarkup.ParagraphXml;
            var endXml = closeMarkup.ParagraphXml;

            if (startXml.Parent != endXml.Parent)
                throw new TemplateEngineException(
                    "Открывающий и закрывающий теги команды foreach должны иметь одного родителя.");

            if (startXml == endXml)
                throw new TemplateEngineException(
                    "Открывающий и закрывающий теги команды foreach не должны совпадать. Т.е. они не могут быть в одном параграфе.");

            var elements = processor.GetElementsBetween(startXml, endXml).ToList();
            if (!elements.Any())
            {
                openMarkup.RemoveMarkup();
                closeMarkup.RemoveMarkup();
                return;
            }

            if (!(dataContext.Get(commandMatch.Groups[2].Value) is IEnumerable enumerable))
                throw new TemplateEngineException("Свойство в конструкции foreach должно быть итерируемое");

            var startIndex = processor.GetParagraphIndexByXml(openMarkup.ParagraphXml) + 1;

            var items = enumerable.Cast<object>().ToArray();
            var markupParagraphIndexMap = markups.ToDictionary(m => m, 
                m => processor.GetParagraphIndexByXml(m.ParagraphXml) - startIndex);
            var paragraphIndexElementMap = new Dictionary<int, XElement>();

            var lastElement = elements.Last();

            var itemParagraphCount = elements.DescendantsAndSelf().Count(p => p.Name.LocalName == "p");
            
            for (int index = 0; index < items.Length; index++)
            {
                var blockItems = elements;
                if (index > 0)
                {
                    blockItems = elements.Select(e => new XElement(e)).ToList();
                    lastElement.AddAfterSelf(blockItems);
                    lastElement = blockItems.Last();
                }
                var paragraphIndex = -1;
                foreach (var copiedItem in blockItems.SelectMany(e=>e.DescendantsAndSelf()))
                {
                    if (copiedItem.Name.LocalName == "p")
                    {
                        ++paragraphIndex;
                        paragraphIndexElementMap.Add(paragraphIndex + index * itemParagraphCount, copiedItem);                        
                    }                    
                }
            }            

            // это очень медленная процедура, нужно вызывать её как можно реже
            //var paragraphs = processor.GetDoc().Paragraphs;

            for (int index = 1; index < items.Length; index++)
            {
                var item = items[index];

                using (BeginItemProcessing(commandMatch, dataContext, index, item))
                {                   
                    var clonedMarkups = new List<Markup>();
                    foreach (var markup in markups)
                    {
                        if (markup.Paragraph.IsListItem)
                        {
                            
                        }
                        var paragraphIndex = markupParagraphIndexMap[markup];
                        var xElement = paragraphIndexElementMap[paragraphIndex + index * itemParagraphCount];                        
                        var p = //paragraphs[startIndex + paragraphIndex + index * itemParagraphCount];                            
                            CreateParagraph(processor.GetDoc(), xElement, markup);
                        
                        var newMarkup = new Markup(markup, p);                     
                        Debug.Assert(newMarkup.ParagraphXml?.Parent != null);
                        clonedMarkups.Add(newMarkup);
                    }

                    var memoryExtractor = new MemoryMarkupExtractor(clonedMarkups);
                    foreach (var markup in memoryExtractor.ExtractMarkups())
                        processor.ApplyMarkup(markup, memoryExtractor, dataContext);
                }
            }

            if (items.Any())
            {
                using (BeginItemProcessing(commandMatch, dataContext, 0, items.First()))
                {
                    var memoryExtractor = new MemoryMarkupExtractor(markups);
                    foreach (var markup in memoryExtractor.ExtractMarkups())
                    {
                        if (markup.Paragraph.IsListItem)
                        {

                        }
                        Debug.Assert(markup.ParagraphXml?.Parent!=null);
                        processor.ApplyMarkup(markup, memoryExtractor, dataContext);
                    }
                }
            }
            else
            {
                foreach (var element in elements)
                    element.Remove();
//                foreach (var markup in markups)
//                    markup.RemoveMarkup();
            }

            openMarkup.RemoveMarkup();
            closeMarkup.RemoveMarkup();
        }

        private Paragraph CreateParagraph(DocX doc, XElement xElement,Markup markup)
        {
            var ctor = typeof(Paragraph).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null,
                new[] {typeof(DocX), typeof(XElement), typeof(int), typeof(ContainerType)}, null);
            var p = (Paragraph) ctor.Invoke(new object[] {doc, xElement, 0, markup.Paragraph.ParentContainer});
            p.PackagePart = doc.PackagePart;
            return p;
        }

        private static ILifetime BeginItemProcessing(Match commandMatch, IDataContext dataContext, int index,
            object item)
        {
            var name = commandMatch.Groups[1].Value;
            if (string.IsNullOrWhiteSpace(name))
                name = null;
            return dataContext.AddIteratingItemToScope(index, item, name);
        }
    }
}