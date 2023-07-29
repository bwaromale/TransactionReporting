

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransactionReportingAPI.Models;

namespace TransactionProcessingService.Models
{
    public class FetchPendingObj
    {
        public int statusCode { get; set; }
        public bool isSuccess { get; set; }
        public string[] errorMessages { get; set; }
        public Transaction[] result { get; set; }
    }
}
