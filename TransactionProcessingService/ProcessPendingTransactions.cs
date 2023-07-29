using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransactionReportingAPI.Data;

namespace TransactionProcessingService
{
    public class ProcessPendingTransactions
    {
        private readonly TransactionProcessingContext _context;

        public ProcessPendingTransactions(TransactionProcessingContext context)
        {
                _context = context;
        }
        public void GetPendingTransations()
        {
            Console.WriteLine("Fetched Pending Transactions");
        }
    }
}
