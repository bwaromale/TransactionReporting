using Newtonsoft.Json;
using TransactionProcessingService.Models;
using TransactionReportingAPI.Models;

namespace TransactionProcessingService
{
    public class Worker : BackgroundService
    {
        LogWriter _logWriter = new();
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logWriter.WriteToLog($"Worker running at: {DateTimeOffset.Now}");
                ProcessPendingTransactions processPending = new();
                processPending.Run();
                await Task.Delay(1000, stoppingToken);
            }
        }
        
    }
}