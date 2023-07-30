using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Transactioneer.Models;

namespace Transactioneer.Controllers
{
    public class TransactionsController : Controller
    {
        private string baseUrl = "https://localhost:7021/api/Transactions/";
        public IActionResult PostTransaction()
        {
            return View();
        }
        [HttpPost]
        public IActionResult PostTransaction(PostTransaction transactionDetails)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(baseUrl);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage res = client.PostAsJsonAsync(baseUrl + "PostTransaction", transactionDetails).Result;

                    if (res.IsSuccessStatusCode)
                    {
                        var result = res.Content.ReadAsStringAsync().Result;
                        PostTransactionResponse postTransactionResponse = JsonConvert.DeserializeObject<PostTransactionResponse>(result);
                        if (postTransactionResponse.isSuccess)
                        {
                            ViewBag.msg = "Transaction Posted Sucessfully";
                            ModelState.Clear();
                        }
                        else
                        {
                            ViewBag.msg = postTransactionResponse.ErrorMessages.ToString();
                            ModelState.Clear();
                        }
                    }
                    else
                    {
                        ViewBag.msg = "Posting failed! Contact Support";
                    }
                }
            }
            catch(Exception ex)
            {
                ViewBag.msg = ex.Message;
            }
            return View();
        }
    }
}
