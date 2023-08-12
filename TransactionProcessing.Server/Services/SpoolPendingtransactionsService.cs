using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using TransactionProcessing.Server.Protos;
using Transaction = TransactionProcessing.Server.Protos.Transaction;
using ModelTransaction = TransactionProcessing.Server.Models.Transaction;
using TransactionProcessing.Server.Data;

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
                var transactions = _dbContext.Transactions.Where(t => t.IsCompleted == false).AsAsyncEnumerable();
                await foreach (ModelTransaction transaction in transactions)
                {
                    var response = MapTransactionToProto(transaction);
                    await responseStream.WriteAsync(response);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(DateTime.Now+ ": Exception at FetchTransactions. Message" + ex.Message);
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
