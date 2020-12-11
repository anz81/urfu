using System.Collections.Generic;

namespace Urfu.Its.VersionedDocs.Core
{
    public interface IVersionedDocumentInspector
    {
        IEnumerable<string> GetErrors();
        IEnumerable<string> GetWarnings();
        void Error(string message);
        void Warning(string message);
        void Info(string message);
        void StopProcessing();
    }
}