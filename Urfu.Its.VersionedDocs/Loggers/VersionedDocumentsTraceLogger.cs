using System;
using System.Diagnostics;
using Urfu.Its.VersionedDocs.Core;

namespace Urfu.Its.VersionedDocs.Loggers
{
    public class VersionedDocumentsTraceLogger<T> : IObjectLogger<T>
    {
        private string PrepareMessage(string logMessage)
        {
            return $"[{typeof(T).Name}]: {logMessage}";
        }

        public void Info(string logMessage, params object[] args)
        {
            Trace.WriteLine("INFO --");
            Trace.WriteLine(PrepareMessage(string.Format(logMessage, args)));
        }

        public void Debug(string logMessage, params object[] args)
        {
            Trace.WriteLine("DEBUG --");
            Trace.WriteLine(PrepareMessage(string.Format(logMessage, args)));
        }

        public void Warning(string logMessage, params object[] args)
        {
            Trace.WriteLine("WARNING --");
            Trace.WriteLine(PrepareMessage(string.Format(logMessage, args)));
        }

        public void Error(string logMessage, params object[] args)
        {
            Trace.WriteLine("ERROR --");
            Trace.WriteLine(PrepareMessage(string.Format(logMessage, args)));
        }

        public void Error(Exception ex)
        {
            Trace.WriteLine("ERROR --");
            Trace.WriteLine(PrepareMessage("Произошла ошибка (детализация ниже):"));
            Trace.WriteLine(ex);
        }
    }
}