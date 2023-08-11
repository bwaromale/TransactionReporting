using Microsoft.EntityFrameworkCore;
using TransactionProcessingService.Models;
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
                .ConfigureServices((hostContext, services) =>
                {
                    var connectionString = hostContext.Configuration.GetConnectionString("DefaultSQLConnection");
                    services.AddDbContext<TransactionProcessingContext>(
                        options =>
                        {
                            options.UseSqlServer(connectionString);
                        });
                    var logDirectory = hostContext.Configuration.GetValue<string>("LogDirectory");
                    services.Configure<Logging>(
                        options =>
                        {
                            options.LogDirectory = logDirectory;
                        });
                    services.AddHostedService<Worker>();
                    services.AddScoped<Logging>();
                })
                .Build();

            host.Run();

        }
        //public static void Main(string[] args)
        //{
        //    CreateHostBuilder(args).Build().Run();
        //}

        //public static IHostBuilder CreateHostBuilder(string[] args) =>
        //    Host.CreateDefaultBuilder(args).
        //        ConfigureAppConfiguration((hostingContext, config) =>
        //        {
        //            config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
        //        })
        //        .ConfigureLogging((hostingContext, logging) =>
        //        {
        //            logging.ClearProviders();
        //            logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
        //            logging.AddProvider(new FileLoggerProvider(
        //                hostingContext.Configuration.GetSection("Logging:FileLogging:FilePath").Value));
        //        })
        //        .ConfigureServices((hostContext, services) =>
        //        {
        //            services.AddHostedService<Worker>();
        //            services.AddSingleton<LogWriter>();
        //        });
    }
}