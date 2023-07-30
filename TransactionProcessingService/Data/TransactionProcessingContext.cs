using Microsoft.EntityFrameworkCore;
using TransactionProcessing.Server.Protos;
using TransactionReportingAPI.Models;
using Transaction = TransactionReportingAPI.Models.Transaction;

namespace TransactionReportingAPI.Data
{
    public class TransactionProcessingContext: DbContext
    {
        public TransactionProcessingContext(DbContextOptions<TransactionProcessingContext> options): base(options)
        {
                
        }
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
            optionsBuilder.UseSqlServer("Server=LAPTOP-BCQ4DB9O\\SQLEXPRESS01; Database=Transactioneer; Trusted_Connection=True;MultipleActiveResultSets=true");
            //IConfigurationRoot configuration = new ConfigurationBuilder()
            //.SetBasePath(Directory.GetCurrentDirectory())
            //.AddJsonFile("appsettings.json")
            //.Build();

            //string connectionString = configuration.GetConnectionString("DefaultSQLConnection");
            //optionsBuilder.UseSqlServer(connectionString);
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
