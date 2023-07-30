using System.ComponentModel.DataAnnotations;

namespace TransactioneerView.Models
{
    public class PostTransaction
    {
        [Required]
        public string SenderRef { get; set; }
        [Required]
        public string ReceiverRef { get; set; }
        [Required]
        public decimal Amount { get; set; }
    }
}
