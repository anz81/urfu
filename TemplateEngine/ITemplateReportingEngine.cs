using System.IO;

namespace TemplateEngine
{
    public interface ITemplateReportingEngine
    {
        void Build<T>(Stream template, T model, Stream output, FileFormat format);
        void Build<T>(string template, T model, string output);
    }
}