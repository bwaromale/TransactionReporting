using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransactionProcessingService.Models
{
    public class FileLoggerProvider : ILoggerProvider
    {
        private readonly string _filePath;

        public FileLoggerProvider(string filePath)
        {
            _filePath = filePath;
        }
        public ILogger CreateLogger(string categoryName)
        {
            return new FileLogger(categoryName);
        }

        public void Dispose()
        {
            
        }
    }
}
