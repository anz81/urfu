using Novacode;
/*using Xceed.Words.NET;

#if XCEED_LICENSED
using Xceed.Document.NET;
#endif*/

namespace TemplateEngine
{
    public class TableCellPosition
    {
        public Table Table { get; set; }
        public Row Row { get; set; }
        public Cell Cell { get; set; }
        public int RowIndex { get; set; }
        public int CellIndex { get; set; }
        public int ParagraphIndex { get; set; }
    }
}