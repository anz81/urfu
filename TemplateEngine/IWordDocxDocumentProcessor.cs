using System.Collections.Generic;
using System.Xml.Linq;
using TemplateEngine.DataContext;
using Novacode;
/*using Xceed.Words.NET;

#if XCEED_LICENSED
using Xceed.Document.NET;
#endif*/

namespace TemplateEngine
{
    public interface IWordDocxDocumentProcessor
    {
        void ApplyMarkup(Markup markup, IMarkupExtractor extractor, IDataContext dataContext);
        Formatting GetFormatting(Paragraph p, int paragraphPosition);
        TableCellPosition GetParagraphPositionInTable(XElement paragraphXml);
        IEnumerable<XElement> GetElementsBetween(XElement startXml, XElement endXml);
        void SetFormatting(Paragraph paragraph, Formatting formatting);            
        DocX GetDoc();
        int GetParagraphIndexByXml(XElement xml);
        void RemoveContentBetween(Paragraph startParagraph, int startIndex, Paragraph endParagraph, int endIndex);
        Markup DuplicateMarkupParagraph(Markup markup, bool autoInsert);
    }
}