namespace TransactionReportingAPI.Models
{
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
}
