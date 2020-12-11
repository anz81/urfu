using System;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Web;
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Layout;
using Microsoft.AspNetCore.Http;
//using System.Runtime.Remoting.Messaging;

namespace Urfu.Its.Common
{
    public class Logger
    {
        private static volatile ILog _log;
        
        public static void Write(TraceEventType logLevel, string message, params object[] args)
        {
            if (_log == null)
                ConfigureAsInGUI();

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == null)
                    args[i] = "null";
            }
            string formattedMessage = string.Format(message, args);
            if (logLevel == TraceEventType.Error || logLevel == TraceEventType.Critical)
                formattedMessage = "*** " + formattedMessage;

            Trace.WriteLine(formattedMessage, "its");
            switch (logLevel)
            {
                case TraceEventType.Verbose:
                    _log.DebugFormat(message, args);
                    break;
                case TraceEventType.Error:
                    _log.ErrorFormat(message, args);
                    break;
                case TraceEventType.Critical:
                    _log.FatalFormat(message, args);
                    break;
                case TraceEventType.Information:
                    _log.InfoFormat(message, args);
                    break;
                case TraceEventType.Warning:
                    _log.WarnFormat(message, args);
                    break;
            }
        }

        public static void Info(string logMessage, params object[] args)
        {
            Write(TraceEventType.Information, logMessage, args);
        }

        public static void Debug(string logMessage, params object[] args)
        {
            Write(TraceEventType.Verbose, logMessage, args);
        }

        public static void Warning(string logMessage, params object[] args)
        {
            Write(TraceEventType.Warning, logMessage, args);
        }

        public static void Error(string logMessage, params object[] args)
        {
            Write(TraceEventType.Error, logMessage, args);
        }

        public static void Error(Exception ex)
        {
            Write(TraceEventType.Error, "Exception: {0}", ex);
        }

        public static void ConfigureAsInGUI()
        {

            _log = LogManager.GetLogger(typeof(Logger));


            var appender = new RollingFileAppender
            {
                AppendToFile = true,
                File =
                    string.Format("{0}\\its.log",
                        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ITS")),
                MaximumFileSize = "3MB",
                MaxSizeRollBackups = 50,
                RollingStyle = RollingFileAppender.RollingMode.Size,
                Threshold = log4net.Core.Level.All,
                LockingModel = new FileAppender.MinimalLock(),
                DatePattern = ".dd.MM.yyyy",
                Layout =
                    new log4net.Layout.PatternLayout(
                        "%date{dd.MM.yyyy HH:mm:ss} [%thread] %-5level - %message%newline"),
            };
            appender.ActivateOptions();

            BasicConfigurator.Configure(appender);
        }

        public static void ConfigureAsInWebService(string logFile = null)
        {

            _log = LogManager.GetLogger(typeof(Logger));


            var appender = new RollingFileAppender
            {
                AppendToFile = true,
                Encoding = Encoding.UTF8,
                File = logFile ?? "its.log",
                MaximumFileSize = "100MB",
                MaxSizeRollBackups = 50,
                RollingStyle = RollingFileAppender.RollingMode.Size,
                Threshold = log4net.Core.Level.All,
                LockingModel = new FileAppender.MinimalLock(),
                DatePattern = ".dd.MM.yyyy",
                Layout =
                    new log4net.Layout.PatternLayout(
                        "%date{dd.MM.yyyy HH:mm:ss} [%thread] %-5level - %message%newline"),
            };

            appender.ActivateOptions();
            BasicConfigurator.Configure(appender);

            TryConfigureDbAppender();
        }

        private static void TryConfigureDbAppender()
        {
            try
            {
                var dbConStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                var appender = new AdoNetAppender
                {
                    BufferSize = 0,
                    Threshold = Level.Info,
                    ConnectionType = "System.Data.SqlClient.SqlConnection",
                    ConnectionString = dbConStr,
                    CommandText =
                        "INSERT INTO Logs ([Date],[Ip],[HttpUser],[Message],[Exception]) VALUES (@log_date, @HTTPIp, @HTTPUser, @message, @exception)",
                };

                appender.AddParameter(new AdoNetAppenderParameter{
                    ParameterName = "@log_date",
                    DbType = DbType.DateTime,
                    Layout = new RawTimeStampLayout()
                });
/*
                appender.AddParameter(new AdoNetAppenderParameter{
                    ParameterName = "@log_level",
                    DbType = DbType.String,
                    Size = 50,
                    Layout = new RawPropertyLayout() { Key = "%level" }
                });*/

                appender.AddParameter(new AdoNetAppenderParameter{
                    ParameterName = "@message",
                    DbType = DbType.String,
                    Size = 8000,
                    Layout = new Layout2RawLayoutAdapter(new PatternLayout("%message")),
                });
                
                appender.AddParameter(new AdoNetAppenderParameter{
                    ParameterName = "@exception",
                    DbType = DbType.String,
                    Size = 8000,
                    Layout = new Layout2RawLayoutAdapter(new ExceptionLayout())
                });

                appender.AddParameter(new AdoNetAppenderParameter
                {
                    ParameterName = "@HTTPUser",
                    DbType = DbType.String,
                    Size = 8000,
                    Layout = new Layout2RawLayoutAdapter(new PatternLayout("%property{user}")),
                });
                appender.AddParameter(new AdoNetAppenderParameter
                {
                    ParameterName = "@HTTPIp",
                    DbType = DbType.String,
                    Size = 8000,
                    Layout = new Layout2RawLayoutAdapter(new PatternLayout("%property{ip}")),
                });

                appender.ActivateOptions();
                BasicConfigurator.Configure(appender);
                log4net.GlobalContext.Properties["user"] = new HttpContextUserNameProvider();
                log4net.GlobalContext.Properties["ip"] = new HttpContextIpProvider();
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }
        }
    }

    public class HttpContextUserNameProvider
    {
        public override string ToString()
        {
            HttpContext context = CallContext.GetData("CurrentContextKey") as HttpContext;
            var host = $"{context.Request.Scheme}://{context.Request.Host}";

            if (context != null && context.User != null && context.User.Identity.IsAuthenticated)
            {
                return context.User.Identity.Name;
            }
            return "";
        }
    }


    public class HttpContextIpProvider
    {
        public override string ToString()
        {
            HttpContext context = CallContext.GetData("CurrentContextKey") as HttpContext;
            if (context == null)
            {
                return "Internal";
            }
            if (context.Request != null)
            {
                String ip = context.Connection.RemoteIpAddress.ToString();
                /*String ip = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

                if (string.IsNullOrEmpty(ip))
                {
                    ip = context.Request.ServerVariables["REMOTE_ADDR"];
                }*/

                return ip;
            }
            return "";
        }
    }
}