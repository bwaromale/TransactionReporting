using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TransactionProcessingService.Models
{
    public class Transaction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TransactionId { get; set; }
        public decimal Amount { get; set; }
        public string SenderRef { get; set; }
        public string ReceiverRef { get; set;}
        public bool IsCompleted { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ProcessedDate { get; set; }

    }
    
}
