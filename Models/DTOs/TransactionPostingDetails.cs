using System.ComponentModel.DataAnnotations;

namespace TransactionReportingAPI.Models.DTOs
{
    public class TransactionPostingDetails
    {
        
        public string SenderRef { get; set; }
       
        public string ReceiverRef { get; set; }
        
        public decimal Amount { get; set; }

        
    }
}
