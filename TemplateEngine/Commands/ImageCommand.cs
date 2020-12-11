using System;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using TemplateEngine.DataContext;

namespace TemplateEngine.Commands
{
    [MarkupCommandPattern(@"^image\s+(?<source>.+?)(?:\s*;\s*(?<width>.+?)\s*;\s*(?<height>.+?)\s*(?:;\s*(?<mode>.+?)\s*)?)?$")]
    public class ImageCommand : MarkupCommand
    {
        public override void Apply(Markup markup, Match commandMatch, IMarkupExtractor extractor, IWordDocxDocumentProcessor processor, IDataContext dataContext)
        {
            var value = dataContext.Get(commandMatch.Groups["source"].Value);

            Size? size = null;
            if (commandMatch.Groups["width"].Success)
            {
                var width = Convert.ToInt32(dataContext.Get(commandMatch.Groups["width"].Value));
                var height = Convert.ToInt32(dataContext.Get(commandMatch.Groups["height"].Value));
                size = new Size(width, height);
            }

            string mode = null;
            if (commandMatch.Groups["mode"].Success)
            {
                mode = Convert.ToString(dataContext.Get(commandMatch.Groups["mode"].Value));
            }

            Stream stream;
            switch (value)
            {
                case Image image:
                    stream = new MemoryStream();
                    image.Save(stream, image.RawFormat);
                    stream.Seek(0, SeekOrigin.Begin);
                    switch (mode?.ToLower())
                    {
                        case "minorfitpreserve":
                            double width = image.Width;
                            double height = image.Height;
                            var s = size.Value;
                            if (width > s.Width)
                            {
                                var k = s.Width / width;
                                width = s.Width;
                                height *= k;
                            }
                            if (height > s.Height)
                            {
                                var k = s.Height / height;
                                height = s.Height;
                                width *= k;
                            }
                            size = new Size((int)width, (int)height);
                            break;
                    }
                    break;
                case string imageDataUrl:
                    var base64Data = Regex.Match(imageDataUrl, @"data:image/(?<type>.+?),(?<data>.+)").Groups["data"].Value;
                    var binData = Convert.FromBase64String(base64Data);
                    stream = new MemoryStream(binData);
                    var imageForSize = Image.FromStream(stream);
                    stream.Seek(0, SeekOrigin.Begin);
                    switch (mode?.ToLower())
                    {
                        case "minorfitpreserve":
                            double width = imageForSize.Width;
                            double height = imageForSize.Height;
                            var s = size.Value;
                            if (width > s.Width)
                            {
                                var k = s.Width / width;
                                width = s.Width;
                                height *= k;
                            }
                            if (height > s.Height)
                            {
                                var k = s.Height / height;
                                height = s.Height;
                                width *= k;
                            }
                            size = new Size((int)width, (int)height);
                            break;
                    }
                    break;
                default:
                    throw new NotSupportedException();
            }

            var img = processor.GetDoc().AddImage(stream);
            markup.InsertImage(img, size);
        }
    }
}