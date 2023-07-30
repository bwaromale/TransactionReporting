namespace TransactionReportingAPI.Models.DTOs
{
    public class TopUpCustomerRequest
    {
        public string CustomerRef { get; set; }
        public decimal Amount { get; set; }
    }
}
