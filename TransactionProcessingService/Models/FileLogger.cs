using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransactionProcessingService.Models
{
    public class FileLogger : ILogger
    {
        private readonly string _filePath;
        private readonly object _lock = new object();

        public FileLogger(string filePath)
        {
            _filePath = filePath;
        }
        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if(!IsEnabled(logLevel))
            {
                return;
            }
            lock(_lock)
            {
                File.AppendAllText(_filePath, $"{DateTime.Now} [{logLevel}] - {formatter(state, exception)}{Environment.NewLine}");
            }
        }
    }
}
