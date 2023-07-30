using Microsoft.EntityFrameworkCore;
using TransactionReportingAPI.Data;

namespace TransactionProcessingService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IHost host = Host.CreateDefaultBuilder(args)
                 .ConfigureAppConfiguration((hostingContext, config) =>
                 {
                     config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                     config.AddEnvironmentVariables();
                 })
                .ConfigureServices((hostContext,services) =>
                {
                    var connectionString = hostContext.Configuration.GetConnectionString("DefaultSQLConnection");
                    services.AddDbContext<TransactionProcessingContext>(
                        options =>
                        {
                            options.UseSqlServer(connectionString);
                        });
                    services.AddHostedService<Worker>();
                })
                .Build();

            host.Run();

        }
    }
}