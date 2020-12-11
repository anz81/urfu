using System;
using System.Diagnostics;
using System.IO;
using Urfu.Its.VersionedDocs.Core;

namespace Urfu.Its.Tools.VersionedDocuments
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

    public class ConsoleLogger<T> : IObjectLogger<T>
    {
        private string PrepareMessage(string logMessage)
        {
            return $"[{typeof(T).Name}]: {logMessage}";
        }

        public void Info(string logMessage, params object[] args)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(PrepareMessage(string.Format(logMessage, args)));
            Console.ResetColor();
        }

        public void Debug(string logMessage, params object[] args)
        {
            Console.WriteLine(PrepareMessage(string.Format(logMessage, args)));
        }

        public void Warning(string logMessage, params object[] args)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(PrepareMessage(string.Format(logMessage, args)));
            Console.ResetColor();
        }

        public void Error(string logMessage, params object[] args)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(PrepareMessage(string.Format(logMessage, args)));
            Console.ResetColor();
        }

        public void Error(Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(PrepareMessage("Произошла ошибка (детализация ниже):"));
            Console.WriteLine(ex);
            Console.ResetColor();
        }
    }

    public class CombinedLogger<T> : IObjectLogger<T>, IDisposable
    {
        private readonly IObjectLogger[] _loggers;

        public CombinedLogger(params IObjectLogger[] loggers)
        {
            _loggers = loggers;
        }

        public void Info(string logMessage, params object[] args)
        {
            foreach (var objectLogger in _loggers)
                objectLogger.Info(logMessage, args);
        }

        public void Debug(string logMessage, params object[] args)
        {
            foreach (var objectLogger in _loggers)
                objectLogger.Debug(logMessage, args);
        }

        public void Warning(string logMessage, params object[] args)
        {
            foreach (var objectLogger in _loggers)
                objectLogger.Warning(logMessage, args);
        }

        public void Error(string logMessage, params object[] args)
        {
            foreach (var objectLogger in _loggers)
                objectLogger.Error(logMessage, args);
        }

        public void Error(Exception ex)
        {
            foreach (var objectLogger in _loggers)
                objectLogger.Error(ex);
        }

        public void Dispose()
        {
            foreach (var objectLogger in _loggers)
            {
                if (objectLogger is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
        }
    }

    public class VersionedDocumentsFileLogger<T> : IObjectLogger<T>, IDisposable
    {
        private readonly string _filePath;
        private readonly StreamWriter _writer;

        public VersionedDocumentsFileLogger(string filePath)
        {
            _filePath = filePath;
            _writer = File.AppendText(_filePath);
        }

        private string PrepareMessage(string logMessage)
        {
            return $"[{typeof(T).Name}]: {logMessage}";
        }

        private void Write(TraceEventType type, string msg)
        {
            string prefix = null;
            switch (type)
            {
                case TraceEventType.Information:
                    prefix = "- INFO ----";
                    break;
                case TraceEventType.Warning:
                    prefix = "- WARNING -";
                    break;
                case TraceEventType.Error:
                    prefix = "- ERROR ---";
                    break;
                case TraceEventType.Verbose:
                    prefix = "- DEBUG ---";
                    break;                
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }

            var timePrefix = $"[{DateTime.Now:HH:mm:ss.fff}]";

            var line = $"{timePrefix} {prefix} {msg}";
            Trace.WriteLine(line);
            _writer.WriteLine(line);
        }

        public void Info(string logMessage, params object[] args)
        {
            Write(TraceEventType.Information, PrepareMessage(string.Format(logMessage, args)));            
        }

        public void Debug(string logMessage, params object[] args)
        {
            Write(TraceEventType.Verbose, PrepareMessage(string.Format(logMessage, args)));            
        }

        public void Warning(string logMessage, params object[] args)
        {
            Write(TraceEventType.Warning, PrepareMessage(string.Format(logMessage, args)));            
        }

        public void Error(string logMessage, params object[] args)
        {
            Write(TraceEventType.Error, PrepareMessage(string.Format(logMessage, args)));            
        }

        public void Error(Exception ex)
        {
            Write(TraceEventType.Error, PrepareMessage(PrepareMessage("Произошла ошибка:\r\n" + ex)));            
        }

        public void Dispose()
        {
            _writer?.Close();
            _writer?.Dispose();
        }
    }
}