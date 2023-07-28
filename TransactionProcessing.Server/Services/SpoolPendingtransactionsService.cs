using Grpc.Core;
using TransactionProcessing.Server.Protos;
using TransactionReportingAPI.Data;

namespace TransactionProcessing.Server.Services
{
    public class SpoolPendingtransactionsService: SpoolPendingTransactions.SpoolPendingTransactionsBase
    {
        private readonly ILogger<SpoolPendingtransactionsService> _logger;

        public SpoolPendingtransactionsService(ILogger<SpoolPendingtransactionsService> logger, TransactionProcessingContext dbContext)
        {
            _logger = logger;
        }
        public override Task FetchTransactions(PendingTransactionsRequest request, IServerStreamWriter<Transactions> responseStream, ServerCallContext context)
        {
            return base.FetchTransactions(request, responseStream, context);
        }
    }
}
