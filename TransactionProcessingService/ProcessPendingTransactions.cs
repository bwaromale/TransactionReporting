using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TransactionProcessingService.Data;
using TransactionProcessingService.Models;

namespace TransactionProcessingService
{
    public class ProcessPendingTransactions
    {
        private readonly TransactionProcessingContext _context;
        private readonly IConfiguration _config;
        private static string baseUrl;
        LogWriter _logWriter = new LogWriter();

        public ProcessPendingTransactions()
        {
            _context = new TransactionProcessingContext(new DbContextOptions<TransactionProcessingContext>());
            _config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
            baseUrl = _config["AppSettings:FetchPendingBaseUrl"];
            
        }

        public void Run()
        {
            List<Transaction> pendingTransactions = GetPendingTransations();
            ProcessPending(pendingTransactions);
        }
        private List<Transaction> GetPendingTransations()
        {
            List<Transaction> pendingTransactions = new List<Transaction>();
            try
            {
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

                        if (fetchPendingResponse.isSuccess == true)
                        {
                            foreach (Transaction transaction in fetchPendingResponse.result)
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
                    else
                    {
                        _logWriter.WriteToLog($"{DateTime.Now}: GetPendingTransations API did not return success");
                    }
                }

            }
            catch (Exception ex)
            {
                _logWriter.WriteExceptionToLog($"{DateTime.Now}: Exception thrown at GetPendingTransations. Message: " +ex.Message);
            }
            return pendingTransactions;
        }

        private void ProcessPending(List<Transaction> transactions)
        {
            try
            {
                foreach (Transaction transaction in transactions)
                {
                    //verify if sender has sufficient balance to perform transaction
                    Customer customer = _context.Customers.Where(c => c.CustomerRef == transaction.SenderRef && c.Balance >= transaction.Amount).FirstOrDefault();
                    if (customer == null)
                    {
                        continue;
                    }
                    //verify transaction id
                    //fetch using id
                    Transaction verifiedPendingTransaction = _context.Transactions.Where(ct => ct.TransactionId == transaction.TransactionId).FirstOrDefault();
                    if (verifiedPendingTransaction == null)
                    {
                        continue;
                    }
                    //debit the sender
                    bool debit = DebitSender(customer, transaction.Amount);
                    //credit the receiver
                    bool credit = CreditReceiver(transaction.ReceiverRef, transaction.Amount);
                    //update isCompleted to true, and set processing date to datetime.now
                    if (debit && credit)
                    {
                        verifiedPendingTransaction.IsCompleted = true;
                        verifiedPendingTransaction.ProcessedDate = DateTime.Now;
                        _context.Update(verifiedPendingTransaction);
                        _context.SaveChanges();
                    }
                }
            }
            catch(Exception ex)
            {
                _logWriter.WriteExceptionToLog($"{DateTime.Now}: Exception thrown at ProcessPending. Message: " + ex.Message);
            }
        }
        private bool DebitSender(Customer customer, decimal amount)
        {
            bool isDebited = false;
            try
            {
                decimal newBalance = customer.Balance - amount;
                customer.Balance = newBalance;

                _context.Update(customer);
                var saveToDb = _context.SaveChanges();
                if (saveToDb == 1)
                {
                    isDebited = true;
                }
            }
            catch (Exception ex)
            {
                _logWriter.WriteExceptionToLog($"{DateTime.Now}: Exception thrown at DebitSender. Message: " + ex.Message);
            }
            return isDebited;
        }
        private bool CreditReceiver(string receiverRef, decimal amount)
        {
            bool isCreditted =  false;
            try
            {
                //fetch receiver
                Customer receiver = _context.Customers.Where(r => r.CustomerRef == receiverRef).FirstOrDefault();
                decimal newBalance = receiver.Balance + amount;

                receiver.Balance = newBalance;
                //update balace
                _context.Update(receiver);
                var saveToDb = _context.SaveChanges();
                if (saveToDb == 1)
                {
                    isCreditted = true;
                }
            }
            catch(Exception ex)
            {
                _logWriter.WriteExceptionToLog($"{DateTime.Now}: Exception thrown at CreditReceiver. Message: " + ex.Message);
            }
            return isCreditted;
        }
    }
}
