using Microsoft.EntityFrameworkCore;
using TransactionReportingAPI.Models;

namespace TransactionReportingAPI.Data
{
    public class TransactionProcessingContext: DbContext
    {
        public TransactionProcessingContext(DbContextOptions<TransactionProcessingContext> options): base(options)
        {
                
        }

        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Customer> Customers { get; set; }
    }
}
