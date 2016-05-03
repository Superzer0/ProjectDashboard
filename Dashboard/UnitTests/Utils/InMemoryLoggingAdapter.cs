using System;
using System.Collections.Generic;
using System.Linq;
using Common.Logging;
using Common.Logging.Simple;

namespace UnitTests.Utils
{
    internal class InMemoryLoggingAdapterFactory : ILoggerFactoryAdapter
    {
        private readonly InMemoryLogger _inMemoryLogger;

        public InMemoryLoggingAdapterFactory(InMemoryLogger inMemoryLogger)
        {
            _inMemoryLogger = inMemoryLogger;
        }

        public static InMemoryLogger CreateDummyLogger()
        {
            return new InMemoryLogger("inMemory", LogLevel.All, true, true, true, "yyyyMMdd");
        }

        public ILog GetLogger(Type type)
        {
            return _inMemoryLogger;
        }

        public ILog GetLogger(string name)
        {
            return _inMemoryLogger;
        }
    }

    public class InMemoryLogger : AbstractSimpleLogger
    {
        private Stack<LogMessage> LoggedExceptions { get; set; }

        public InMemoryLogger(string logName, LogLevel logLevel, bool showlevel, bool showDateTime,
            bool showLogName, string dateTimeFormat) : base(logName, logLevel, showlevel, showDateTime, showLogName, dateTimeFormat)
        {
            LoggedExceptions = new Stack<LogMessage>();
        }
        public List<LogMessage> GetLoggedMessages()
        {
            return LoggedExceptions.ToList();
        }

        protected override void WriteInternal(LogLevel level, object message, Exception exception)
        {
            LoggedExceptions.Push(new LogMessage
            {
                Level = level,
                Message = message,
                Exception = exception
            });
        }
    }

    public class LogMessage
    {
        public LogLevel Level { get; set; }
        public object Message { get; set; }
        public Exception Exception { get; set; }
    }
}
