using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
//using Xceed.Words.NET;
using Novacode;
using Image = Novacode.Image;
/*#if !XCEED_LICENSED
using Image = Xceed.Words.NET.Image;
#endif

#if XCEED_LICENSED
using Xceed.Document.NET;
using Image = Xceed.Document.NET.Image;
#endif
*/
namespace TemplateEngine
{
    public class Markup
    {
        public Markup()
        {

        }

        public Markup(Markup copyFromMarkup, Paragraph newParagraph)
        {
            Paragraph = newParagraph;
            Index = copyFromMarkup.Index;
            MarkupText = copyFromMarkup.MarkupText;
            MarkupPattern = copyFromMarkup.MarkupPattern;
            MarkupMatch = copyFromMarkup.MarkupMatch;
            Formatting = copyFromMarkup.Formatting;
            CommandText = copyFromMarkup.CommandText;
            ReplaceMark = copyFromMarkup.ReplaceMark;
        }

        public Paragraph Paragraph { get; set; }
        public XElement ParagraphXml => Paragraph.Xml;
        public int Index { get; set; }
        public string MarkupText { get; set; }
        public string MarkupPattern { get; set; }
        public Match MarkupMatch { get; set; }
        public Formatting Formatting { get; set; }
        public string CommandText { get; set; }
        //public XElement OriginalParagraphXml { get; set; }
        public string ReplaceMark { get; set; }

        public void RemoveMarkup()
        {
            //Paragraph.RemoveText(Index, ReplaceMark.Length);
            // Немного увеличивает производительность
            foreach (var e in ParagraphXml.Descendants().Where(d => !d.HasElements))
            {
                var index = e.Value.IndexOf(ReplaceMark);
                if (index >= 0)
                {
                    e.Value = e.Value.Remove(index, ReplaceMark.Length);
                    var tcAncestor = ParagraphXml.Ancestors().FirstOrDefault(a => a.Name.LocalName == "tc");
                    if (string.IsNullOrEmpty(Paragraph.Text))
                    {
                        if (tcAncestor == null || tcAncestor.Descendants().Count(el => el.Name.LocalName == "p") > 1)
                            Paragraph.Remove(false);
                    }
                    return;
                }
            }

            Paragraph.ReplaceText(ReplaceMark, "");
        }

        public void InsertText(string text)
        {
            //var paragraphs = text.Split(new[]{'\n'}, StringSplitOptions.None);
            
            //Paragraph.ReplaceText(ReplaceMark, text);   
            // Значительно увеличивает производительность!
            foreach (var e in ParagraphXml.Descendants().Where(d => !d.HasElements))
            {
                var index = e.Value.IndexOf(ReplaceMark);
                if (index >= 0)
                {
                    var val = e.Value.Insert(index + ReplaceMark.Length, text);
                    val = val.Remove(index, ReplaceMark.Length);
                    e.SetValue(val);                    
                    break;
                }
            }

            Paragraph.ReplaceText(ReplaceMark, text, false, RegexOptions.None, Formatting);            
            //            Paragraph.InsertText(Index + ReplaceMark.Length, text, false, Formatting);
            //            Paragraph.RemoveText(Index, ReplaceMark.Length);         
        }

        public void InsertImage(Image img, Size? size = null)
        {
            Picture picture;
            if (size != null)
                picture = img.CreatePicture(size.Value.Height, size.Value.Width);
            else
                picture = img.CreatePicture();
//#if XCEED_LICENSED
            //picture.WrappingStyle = PictureWrappingStyle.WrapTopAndBottom;
            //picture.VerticalOffsetAlignmentFrom = PictureVerticalOffsetAlignmentFrom.Line;
//#endif            
            Paragraph.InsertPicture(picture, Index);
            Paragraph.ReplaceText(ReplaceMark, "");            
        }

        public override string ToString()
        {
            return MarkupText;
        }
    }
}