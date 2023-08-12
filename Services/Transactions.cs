using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Options;
using TransactionProcessing.Server.Protos;
using TransactionReportingAPI.Data;
using TransactionReportingAPI.Models;
using TransactionReportingAPI.Models.DTOs;
using Transaction = TransactionReportingAPI.Models.Transaction;

namespace TransactionReportingAPI.Services
{
    public class Transactions : ITransactions
    {
        private readonly GrpcServer _grpcServer;
        private readonly TransactionProcessingContext _db;
        private readonly SpoolPendingTransactions.SpoolPendingTransactionsClient _grpcClient;

        public Transactions(TransactionProcessingContext db, IOptions<GrpcServer> grpcServer)
        {
            _db = db;
            _grpcServer = grpcServer.Value;

            GrpcChannel channel = GrpcChannel.ForAddress(_grpcServer.GrpcServerHost);
            _grpcClient = new SpoolPendingTransactions.SpoolPendingTransactionsClient(channel);
        }
        public async Task<List<Transaction>> GetPendingTransactions()
        {
            List<Transaction> transactions = new List<Transaction>();
            try
            {
                var input = new PendingTransactionsRequest();
                var respFrmGrpc = _grpcClient.FetchTransactions(input);

                await foreach (var streamedTransaction in respFrmGrpc.ResponseStream.ReadAllAsync())
                {
                    Transaction transaction = new Transaction();
                    transaction.TransactionId = streamedTransaction.TransactionId;
                    transaction.SenderRef = streamedTransaction.SenderRef;
                    transaction.ReceiverRef = streamedTransaction.ReceiverRef;
                    transaction.Amount = streamedTransaction.Amount;
                    transaction.CreateDate = streamedTransaction.CreateDate.ToDateTime();
                    transactions.Add(transaction);
                }
            }
            catch (Exception ex)
            {

            }
            return transactions;
        }

        public async Task<List<TransactionReport>> GetTransactionsReport()
        {
            List<TransactionReport> allTransactionsReport = new List<TransactionReport>();
            try
            {
                allTransactionsReport =   _db.Transactions
                                    .Select(b => new TransactionReport
                                    {
                                        TransactionId = b.TransactionId,
                                        SenderName = _db.Customers
                                            .Where(c => c.CustomerRef == b.SenderRef)
                                            .Select(c => c.CustomerName)
                                            .FirstOrDefault(),
                                        SenderRef = b.SenderRef,
                                        ReceiverName = _db.Customers
                                            .Where(c => c.CustomerRef == b.ReceiverRef)
                                            .Select(c => c.CustomerName)
                                            .FirstOrDefault(),
                                        ReceiverRef = b.ReceiverRef,
                                        Amount = b.Amount,
                                        PostedOn = b.CreateDate,
                                        ProcessedOn = b.ProcessedDate,
                                        Completed = b.IsCompleted
                                    })
                                    .OrderBy(b => b.TransactionId)
                                    .ToList();
                
            }
            catch(Exception ex)
            {

            }
            return allTransactionsReport;
        }

        public Transaction PostTransactionToLog(TransactionPostingDetails postingDetails)
        {
            Transaction newTransaction = new Transaction();
            try
            {
                //assign values to needed variables
                
                newTransaction.Amount = postingDetails.Amount;
                newTransaction.SenderRef = postingDetails.SenderRef;
                newTransaction.ReceiverRef = postingDetails.ReceiverRef;
                newTransaction.CreateDate = DateTime.Now;
                newTransaction.IsCompleted = false;

                //addtodb
                _db.Transactions.Add(newTransaction);
                _db.SaveChanges();
            }
            catch(Exception ex)
            {

            }
            return newTransaction;
        }

        public bool TopUpBalance(string customerRef, decimal amount)
        {
            bool isSuccessful = false;
            try
            {
                Customer customer = _db.Customers.Where(c => c.CustomerRef == customerRef).FirstOrDefault();
                decimal newBalance = customer.Balance + amount;
                customer.Balance = newBalance;
                _db.Update(customer);
                int updateBalance = _db.SaveChanges();
                if (updateBalance == 1)
                {
                    isSuccessful = true;
                }
            }
            catch(Exception ex)
            {

            }
            return isSuccessful;
        }

        public bool VerifyAccountBalance(string senderRef, decimal transAmount)
        {
            bool isSufficient = false;
            var sender = _db.Customers.Where(s => s.CustomerRef == senderRef && s.Balance >= transAmount).FirstOrDefault();
            if (sender != null)
            {
                isSufficient = true;
            }

            return isSufficient;
        }

        public bool VerifyReference(string customerRef)
        {
            bool isExist = false;
            var verify =  _db.Customers.Where(v => v.CustomerRef == customerRef).FirstOrDefault();

            if (verify != null)
            {
                isExist = true;
            }

            return isExist;
        }
    }
}
