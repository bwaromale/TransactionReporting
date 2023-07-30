using Microsoft.AspNetCore.Mvc;

namespace TransactioneerView.Controllers
{
    public class TransactionsController : Controller
    {
        public IActionResult PostTransaction()
        {
            return View();
        }
        public IActionResult TransactionsReport()
        {
            return View();
        }
    }
}
