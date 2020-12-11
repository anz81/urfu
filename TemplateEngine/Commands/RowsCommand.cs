using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using TemplateEngine.DataContext;

namespace TemplateEngine.Commands
{
    [MarkupCommandPattern(@"^(?<commandName>rows)\s+(?:([a-z0-9_]+)\s+in\s+)?(.+)$", RegexOptions.IgnoreCase)]
    public class RowsCommand : MarkupCommand
    {
        public override void Apply(Markup openMarkup, Match commandMatch, IMarkupExtractor extractor, IWordDocxDocumentProcessor processor, IDataContext dataContext)
        {
            var cellMarkups = extractor.ExtractNestedMarkups(commandMatch.Groups["commandName"].Value, out var closeMarkup).ToList();

            var startColumnIndex = processor.GetParagraphPositionInTable(openMarkup.ParagraphXml).CellIndex;
            var endColumnIndex = processor.GetParagraphPositionInTable(closeMarkup.ParagraphXml).CellIndex;

            openMarkup.RemoveMarkup();
            closeMarkup.RemoveMarkup();

            var cells = cellMarkups.ToDictionary(m => m, m => processor.GetParagraphPositionInTable(m.ParagraphXml));

            var table = cells.Values.First().Table;
            var startRowIndex = cells.Values.First().RowIndex;
            
            if (!(dataContext.Get(commandMatch.Groups[2].Value) is IEnumerable enumerable))
                throw new TemplateEngineException("Свойство в конструкции rows должно быть итерируемое");

            var templateRow = table.Rows[startRowIndex];

            var items = enumerable.Cast<object>().ToArray();
            for (var index = 1; index < items.Length; index++)
            {
                var rowIndex = startRowIndex + index;
                var item = items[index];
                using (BeginItemProcessing(commandMatch, dataContext, index, item))
                {
                    var row = table.InsertRow(templateRow, rowIndex);
                    var vMerges = row.Xml.Descendants().Where(e => e.Name.LocalName == "vMerge").ToList();                   
                    vMerges.Attributes().Where(a => a.Name.LocalName == "val").Remove();

                    var clonedMarkups = new List<Markup>();
                    foreach (var cell in cells)
                    {
                        var paragraph = row.Cells[cell.Value.CellIndex].Paragraphs[cell.Value.ParagraphIndex];                        
                        var markup = new Markup(cell.Key, paragraph);
                        clonedMarkups.Add(markup);
                    }

                    var rowCountBuffer = table.RowCount;
                    var memoryExtractor = new MemoryMarkupExtractor(clonedMarkups);
                    foreach (var markup in memoryExtractor.ExtractMarkups())
                    {
                        processor.ApplyMarkup(markup, memoryExtractor, dataContext);                        
                    }
                    startRowIndex += table.RowCount - rowCountBuffer;

                    //rowOpenMarkup.RemoveMarkup();
                }                
            }

            if (items.Any())
            {
                using (BeginItemProcessing(commandMatch, dataContext, 0, items.First()))
                {
                    var memoryExtractor = new MemoryMarkupExtractor(cellMarkups);
                    foreach (var markup in memoryExtractor.ExtractMarkups())
                        processor.ApplyMarkup(markup, memoryExtractor, dataContext);
                }
            }
            else
            {
                foreach (var markup in cellMarkups)
                    markup.RemoveMarkup();
            }

            var endRow = startRowIndex + items.Length - 1;
            if (endRow > startRowIndex)
            {
                foreach (var cellIndex in Enumerable.Range(0, startColumnIndex)
                    .Concat(Enumerable.Range(endColumnIndex + 1, table.ColumnCount - endColumnIndex - 1)))
                {
                    var cell = templateRow.Cells[cellIndex];
                    if (cell.Xml.Descendants().All(e => e.Name.LocalName != "vMerge"))
                    {
                        table.MergeCellsInColumn(cellIndex, startRowIndex, endRow);
                    }
                }
            }
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