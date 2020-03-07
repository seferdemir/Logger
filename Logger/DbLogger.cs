using Logger.Data;
using Logger.Models;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Reflection;

namespace Logger
{
    public class DBLogger : Logger.Interface.ILogger, ILogger
    {
        private string categoryName;
        private Func<string, LogLevel, bool> _filter;
        private LoggerContext context;
        private bool selfException = false;

        public DBLogger(string categoryName)
        {
            this.categoryName = categoryName;
            context = new LoggerContext();
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }
            if (selfException)
            {
                selfException = false;
                return;
            }
            selfException = true;
            if (formatter == null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }
            var message = formatter(state, exception);
            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            if (exception != null)
            {
                message += "\n" + exception.ToString();
            }

            Add(message, eventId);
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return (_filter == null || _filter(categoryName, logLevel));
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        private int? GetMaxMessageLength()
        {
            int? maxLength = null;
            PropertyInfo[] props = typeof(EventLog).GetProperties();
            foreach (PropertyInfo prop in props)
            {
                object[] attrs = prop.GetCustomAttributes(true);
                foreach (object attr in attrs)
                {
                    MaxLengthAttribute maxLengthAttr = attr as MaxLengthAttribute;
                    if (maxLengthAttr != null && prop.Name.Equals("Message"))
                    {
                        maxLength = maxLengthAttr.Length;
                    }
                }
            }

            return maxLength;
        }

        public void Add(string message, object obj)
        {
            try
            {
                var maxMessageLength = GetMaxMessageLength();
                message = maxMessageLength != null && message.Length > maxMessageLength ? message.Substring(0, (int)maxMessageLength) : message;
                context.EventLogs.Add(new EventLog { Message = message, Value = obj.ToString(), CreatedTime = DateTime.UtcNow });
                context.SaveChanges();
                selfException = false;
            }
            catch (Exception ex)
            {
                using (var streamWriter = new StreamWriter("C:\\log\\AspCoreFileLog.txt", true))
                {
                    streamWriter.WriteLine(string.Format("Created Time: {0} | Message: {1} | Value: {2} \nException => {3}", DateTime.Now.ToString(), message, obj.ToString(), ex.ToString()));
                    streamWriter.Close();
                }
            }
        }
    }
}
