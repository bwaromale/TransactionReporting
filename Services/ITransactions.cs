using TransactionReportingAPI.Models;
using TransactionReportingAPI.Models.DTOs;

namespace TransactionReportingAPI.Services
{
    public interface ITransactions
    {
        Task<List<TransactionReport>> GetTransactionsReport();
        Task<List<Transaction>> GetPendingTransactions();
        bool VerifyReference(string customerRef);
        bool VerifyAccountBalance(string senderRef, decimal transAmount);
        bool TopUpBalance(string customerRef, decimal amount);
        Transaction PostTransactionToLog(TransactionPostingDetails postingDetails);
    }
}
