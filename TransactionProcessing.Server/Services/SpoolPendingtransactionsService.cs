using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using TransactionProcessing.Server.Protos;
using TransactionReportingAPI.Data;
using TransactionReportingAPI.Models;
using Transaction = TransactionProcessing.Server.Protos.Transaction;
using ModelTransaction = TransactionReportingAPI.Models.Transaction;

namespace TransactionProcessing.Server.Services
{
    public class SpoolPendingtransactionsService: SpoolPendingTransactions.SpoolPendingTransactionsBase
    {
        private readonly ILogger<SpoolPendingtransactionsService> _logger;
        private readonly TransactionProcessingContext _dbContext;

        public SpoolPendingtransactionsService(ILogger<SpoolPendingtransactionsService> logger, TransactionProcessingContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }
        public override async Task FetchTransactions(PendingTransactionsRequest request, IServerStreamWriter<Transaction> responseStream, ServerCallContext context)
        {
            try
            {
                var transactions = _dbContext.Transactions.AsAsyncEnumerable();
                await foreach (ModelTransaction transaction in transactions)
                {
                    var response = MapTransactionToProto(transaction);
                    await responseStream.WriteAsync(response);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }
        private Transaction MapTransactionToProto(ModelTransaction transaction)
        {
            return new Transaction
            {
                TransactionId = transaction.TransactionId,
                Amount = Convert.ToInt64(transaction.Amount),
                SenderRef = transaction.SenderRef,
                ReceiverRef = transaction.ReceiverRef,
                IsCompleted = transaction.IsCompleted,
                //CreateDate = Timestamp.FromDateTime(transaction.CreateDate),
                CreateDate = Timestamp.FromDateTime(transaction.CreateDate.ToUniversalTime()),
                ProcessedDate = Timestamp.FromDateTime(transaction.ProcessedDate.ToUniversalTime())
            };
        }

    }
}
