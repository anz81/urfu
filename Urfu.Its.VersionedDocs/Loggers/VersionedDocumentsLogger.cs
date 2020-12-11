using System;
using Urfu.Its.Common;
using Urfu.Its.VersionedDocs.Core;

namespace Urfu.Its.VersionedDocs.Loggers
{
    public class VersionedDocumentsLogger<T> : IObjectLogger<T>
    {
        private string PrepareMessage(string logMessage)
        {
            return $"{logMessage}";
        }

        public void Info(string logMessage, params object[] args)
        {
            Logger.Info(PrepareMessage(logMessage), args);
        }

        public void Debug(string logMessage, params object[] args)
        {
            Logger.Debug(PrepareMessage(logMessage), args);
        }

        public void Warning(string logMessage, params object[] args)
        {
            Logger.Warning(PrepareMessage(logMessage), args);
        }

        public void Error(string logMessage, params object[] args)
        {
            Logger.Error(PrepareMessage(logMessage), args);
        }

        public void Error(Exception ex)
        {
            Logger.Error(PrepareMessage("Произошла ошибка (детализация ниже):"));
            Logger.Error(ex);
        }
    }
}