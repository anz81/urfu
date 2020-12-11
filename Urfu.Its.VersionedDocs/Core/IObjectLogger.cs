using System;

namespace Urfu.Its.VersionedDocs.Core
{
    public interface IObjectLogger<T> : IObjectLogger
    {
        
    }

    public interface IObjectLogger
    {
        void Info(string logMessage, params object[] args);
        void Debug(string logMessage, params object[] args);
        void Warning(string logMessage, params object[] args);
        void Error(string logMessage, params object[] args);
        void Error(Exception ex);
    }
}