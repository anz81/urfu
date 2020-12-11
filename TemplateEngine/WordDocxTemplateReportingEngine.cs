using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
//using PdfConverter;
using TemplateEngine.DataContext;
using Novacode;
/*using Xceed.Words.NET;

#if XCEED_LICENSED
using Xceed.Document.NET;
#endif*/

namespace TemplateEngine
{
    public class WordDocxTemplateReportingEngine : ITemplateReportingEngine
    {
        static WordDocxTemplateReportingEngine()
        {
            //Xceed.Words.NET.Licenser.LicenseKey = "WDN13-GMW1T-8T445-JX6A";
        }

        public void Build<T>(Stream template, T model, Stream output, FileFormat format)
        {
            using (var doc = DocX.Load(template))
            {
                IWordDocxDocumentProcessor processor = new WordDocxDocumentProcessor(doc);

                //Stopwatch sw = Stopwatch.StartNew();
                var debugMode = format == FileFormat.Docx;
                var markups = new DocxMarkupExtractor(doc, processor, debugMode).ExtractMarkups().ToList();
                //sw.Stop();
                //Console.WriteLine(sw.Elapsed);
                var memoryExtractor = new MemoryMarkupExtractor(markups);
                
                var evaluator = new DynamicLinqExpressionEvaluator();
                var dataContext = new DocumentDataContext(evaluator);
                using (dataContext.Change(new ScopeChangeDescriptor(model)))
                {
                    foreach (var markup in memoryExtractor.ExtractMarkups())
                    {
                        try
                        {
                            processor.ApplyMarkup(markup, memoryExtractor, dataContext);
                        }
                        catch (Exception ex)
                        {
                            Trace.WriteLine(markup + ": " + ex);
                        }
                    }
                }

                //doc.AddPasswordProtection(EditRestrictions.readOnly, "test");

                if (format == FileFormat.Docx)
                {
                    doc.SaveAs(output);
                }
                else if (format == FileFormat.LockedDocx)
                {
                    doc.AddProtection(EditRestrictions.readOnly, Guid.NewGuid().ToString());
                    doc.SaveAs(output);
                }
                else if (format == FileFormat.Pdf)
                {
                    throw new InvalidOperationException("Необходима лицензионная версия компонента Xceed.Words.NET");
                    var stream = new MemoryStream();
                    doc.SaveAs(stream);
                    stream.Seek(0, SeekOrigin.Begin);
                    //Converter.FromDocx(stream, output);
//#if XCEED_LICENSED
                    //Document.ConvertToPdf(doc, output);
//#else
                    //DocX.ConvertToPdf(doc, "D:\\test.pdf");
                    //Document.ConvertToPdf(doc, output);
                    //throw new InvalidOperationException("Необходиа лицензионная версия компонента Xceed.Words.NET");
//#endif
                }
                else
                {
                    throw new InvalidOperationException("Формат не поддерживается");
                }
            }
        }

        public void Concat(Stream first, Stream second, Stream output)
        {
            using (var doc = DocX.Load(first))
            {
                var newDoc = DocX.Load(second);

                doc.InsertParagraph().InsertPageBreakAfterSelf();
                doc.InsertDocument(newDoc);

                doc.SaveAs(output);
            }
        }

        public void Build<T>(string template, T model, string output)
        {
            var fileExtension = Path.GetExtension(output);
            using (var stream = File.OpenRead(template))
            {
                using (var outputStream = File.OpenWrite(output))
                {
                    FileFormat format;
                    if (fileExtension == ".docx")
                        format = FileFormat.Docx;
                    else if (fileExtension == ".pdf")
                        format = FileFormat.Pdf;
                    else
                        throw new InvalidOperationException("Формат не поддерживается");
                    Build(stream, model, outputStream, format);
                }
            }
        }
        public void SetKeepNextParagraph(MemoryStream output, string textparagraph)
        {
            using (var doc = DocX.Load(output))
            {
                bool nextParagraphersWillBeKeepToogether = false;
                foreach (var p in doc.Paragraphs)
                {                   
                    if (p.Text.Contains(textparagraph) || nextParagraphersWillBeKeepToogether)
                    {
                        nextParagraphersWillBeKeepToogether = true;
                        p.KeepWithNext(true);
                    }

                }
                doc.Save();
            }
        }

    }

    public enum FileFormat
    {
        Docx,
        LockedDocx,
        Pdf
    }
}