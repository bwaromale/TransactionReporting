using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using TransactionReportingAPI.Data;
using TransactionReportingAPI.Models;
using TransactionReportingAPI.Models.DTOs;

namespace TransactionReportingAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly TransactionProcessingContext _db;
        protected APIResponse _response = new APIResponse();
        public TransactionsController(TransactionProcessingContext db)
        {
             _db = db;
        }
        [HttpPost]
        public ActionResult<APIResponse> PostTransaction(TransactionPostingDetails details)
        {
            try
            {
                if (details == null || details.Amount == 0)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.ErrorMessages.Add("Details provided are not supported for processing by System. Kindly review!");
                    return _response;
                }
                bool senderRefOk = VerifyReference(details.SenderRef);
                bool receiverRefOk = VerifyReference(details.ReceiverRef);
                if (!senderRefOk || !receiverRefOk)
                {
                    string forSenderRefOk = senderRefOk ? "Sender Reference Verified" : "Sender Reference does not exist";
                    string forReceiverRefOk = receiverRefOk ? "Receiver Reference Verified" : "Receiver Reference does not exist";
                    _response.IsSuccess=false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.ErrorMessages.Add(forSenderRefOk);
                    _response.ErrorMessages.Add(forReceiverRefOk);
                    return _response;
                }

                bool isBalanceSufficient = VerifyAccountBalance(details.SenderRef, details.Amount);
                if (!isBalanceSufficient)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.ErrorMessages.Add("Your balance is lower than the amount you are sending! Kindly top up your balance!");
                    return _response;
                }

                //assign values to needed variables
                var newTransaction = new Transaction();
                newTransaction.Amount = details.Amount;
                newTransaction.SenderRef = details.SenderRef;
                newTransaction.ReceiverRef = details.ReceiverRef;
                newTransaction.CreateDate = DateTime.Now;
                newTransaction.IsCompleted = false;

                //addtodb
                _db.Transactions.Add(newTransaction);
                _db.SaveChanges();



                _response.StatusCode = HttpStatusCode.OK;
                _response.Result = newTransaction;
                
            }
            catch (Exception ex) 
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add(ex.Message);
            }
            return _response;
        }
        private bool VerifyReference(string customerRef)
        {
            bool isExist = false;
            var verify = _db.Customers.Where(v => v.CustomerRef == customerRef).FirstOrDefault();

            if(verify != null) 
            {
                isExist = true;
            }

            return isExist;
        }
        private bool VerifyAccountBalance(string senderRef, decimal transAmount)
        {
            bool isSufficient = false;
            var sender = _db.Customers.Where(s => s.CustomerRef == senderRef && s.Balance >= transAmount).FirstOrDefault();
            if(sender != null)
            {
                isSufficient = true;
            }

            return isSufficient;
        }
    }
}
