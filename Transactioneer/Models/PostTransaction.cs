namespace Transactioneer.Models
{
    public class Transaction
    {
        public int TransactionId { get; set; }
        public decimal Amount { get; set; }
        public string SenderRef { get; set; }
        public string ReceiverRef { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ProcessedDate { get; set; }

    }
    public class PostTransaction
    {
        public string SenderRef { get; set; }
        public string ReceiverRef { get; set; }
        public decimal Amount { get; set; }
    }
    public class PostTransactionResponse
    {
        public int StatusCode { get; set; }
        public bool isSuccess { get; set; }
        public string[] ErrorMessages { get; set; }
        public Transaction Result { get; set; }

    }
    public class TransactionReport
    {
        public int TransactionId { get; set; }
        public string SenderName { get; set; }
        public string SenderRef { get; set; }
        public string ReceiverName { get; set; }
        public string ReceiverRef { get; set; }
        public decimal Amount { get; set; }
        public DateTime PostedOn { get; set; }
        public DateTime ProcessedOn { get; set; }
        public bool Completed { get; set; }
    }
    public class TransactionsReportResponse
    {
        public int StatusCode { get; set; }
        public bool IsSuccess { get; set; }
        public string[] ErrorMessages { get; set; }
        public TransactionReport[] Result { get; set; }
    }
}
