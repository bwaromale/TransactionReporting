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
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>()
                .HasIndex(c => c.CustomerRef)
                .IsUnique();
        }
    }
}
