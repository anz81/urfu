using System.Collections.Generic;
using System.Diagnostics;
using Urfu.Its.VersionedDocs.Core;

namespace Urfu.Its.VersionedDocs.Services
{
    public class LoggingVersionedDocumentInspector : IVersionedDocumentInspector
    {
        private readonly IObjectLogger<LoggingVersionedDocumentInspector> _logger;

        private readonly Dictionary<TraceEventType, List<string>> _messages = new Dictionary<TraceEventType, List<string>>()
        {
            { TraceEventType.Error, new List<string>() },
            { TraceEventType.Warning, new List<string>() },
            { TraceEventType.Verbose, new List<string>() },
        };

        public LoggingVersionedDocumentInspector(IObjectLogger<LoggingVersionedDocumentInspector> logger)
        {
            _logger = logger;
        }

        public IEnumerable<string> GetErrors()
        {
            return _messages[TraceEventType.Error];
        }

        public IEnumerable<string> GetWarnings()
        {
            return _messages[TraceEventType.Warning];
        }

        public void Error(string message)
        {
            _messages[TraceEventType.Error].Add(message);
            _logger.Debug(message);
        }

        public void Warning(string message)
        {
            _messages[TraceEventType.Warning].Add(message);
            _logger.Debug(message);
        }

        public void Info(string message)
        {
            _messages[TraceEventType.Verbose].Add(message);
            _logger.Debug(message);
        }

        public void StopProcessing()
        {
            throw new VersionedDocumentInspectorStopProcessingException(GetErrors(), GetWarnings());
        }
    }
}