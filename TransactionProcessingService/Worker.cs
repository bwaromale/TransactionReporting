using Newtonsoft.Json;
using TransactionProcessingService.Models;
using TransactionReportingAPI.Models;

namespace TransactionProcessingService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private string baseUrl = "https://localhost:7021/api/Transactions/";
        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                GetPendingTransations();
                await Task.Delay(1000, stoppingToken);
            }
        }
        public List<Transaction> GetPendingTransations()
        {

            List<Transaction> pendingTransactions = new List<Transaction>();
            
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage res = client.GetAsync(baseUrl + "GetPendingTransactions").Result;

                if (res.IsSuccessStatusCode)
                {
                    var result = res.Content.ReadAsStringAsync().Result;
                    FetchPendingObj fetchPendingResponse = JsonConvert.DeserializeObject<FetchPendingObj>(result);

                    if(fetchPendingResponse.isSuccess == true)
                    {
                        foreach(Transaction transaction in fetchPendingResponse.result)
                        {
                            Transaction pendingTransaction = new Transaction();

                            pendingTransaction.TransactionId = transaction.TransactionId;
                            pendingTransaction.Amount = transaction.Amount;
                            pendingTransaction.SenderRef = transaction.SenderRef;
                            pendingTransaction.ReceiverRef = transaction.ReceiverRef;
                            pendingTransaction.CreateDate = transaction.CreateDate;
                            pendingTransaction.ProcessedDate = transaction.ProcessedDate;
                            pendingTransaction.IsCompleted = transaction.IsCompleted;

                            pendingTransactions.Add(pendingTransaction);
                        }
                    }
                }
            } 
            return pendingTransactions;
        }

    }
}