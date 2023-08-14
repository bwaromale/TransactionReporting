using Newtonsoft.Json;
using TransactionProcessingService.Models;
using TransactionReportingAPI.Models;

namespace TransactionProcessingService
{
    public class Worker : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                ProcessPendingTransactions processPending = new();
                processPending.Run();
                await Task.Delay(1000, stoppingToken);
            }
        }
        
    }
}