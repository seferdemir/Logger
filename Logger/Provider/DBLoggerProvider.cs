using Microsoft.Extensions.Logging;

namespace Logger.Provider
{
    public class DBLoggerProvider : ILoggerProvider
    {
        public ILogger CreateLogger(string categoryName)
        {
            return new DBLogger(categoryName);
        }

        public void Dispose()
        {
        }
    }
}
