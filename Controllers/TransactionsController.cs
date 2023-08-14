using Microsoft.AspNetCore.Mvc;
using System.Net;
using TransactionReportingAPI.Data;
using TransactionReportingAPI.Models;
using TransactionReportingAPI.Models.DTOs;
using TransactionReportingAPI.Services;
using Transaction = TransactionReportingAPI.Models.Transaction;

namespace TransactionReportingAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactions _transactionsService;
        protected APIResponse _response = new();
        public TransactionsController(ITransactions transactionsService)
        {
            _transactionsService = transactionsService;
        }
        [HttpGet]
        public async Task<ActionResult<APIResponse>> GetPendingTransactions()
        {
            List<Transaction> transactions = await _transactionsService.GetPendingTransactions();

            _response.ResponseCode = "00";
            _response.Result = transactions;
            return _response;
        }
        [HttpGet]
        public async Task<ActionResult<APIResponse>> GetTransactionsReport()
        {
            var allTransactionsReport = await _transactionsService.GetTransactionsReport();

            _response.ResponseCode = "00";
            _response.Result = allTransactionsReport;
            return _response;
        }
        [HttpPost]
        public ActionResult<APIResponse> TopUpBalance(TopUpCustomerRequest topUpDetails)
        {
            if (topUpDetails.CustomerRef == null || topUpDetails.Amount == 0)
            {
                _response.IsSuccess = false;
                _response.ResponseCode = "01";
                _response.ErrorMessages.Add("Invalid top up request. Review details!");
                return _response;
            }

            bool customerExist = _transactionsService.VerifyReference(topUpDetails.CustomerRef);
            if (!customerExist)
            {
                _response.IsSuccess = false;
                _response.ResponseCode = "01";
                _response.ErrorMessages.Add($"Customer reference '{topUpDetails.CustomerRef}' provided for Top Up  is invalid");
                return _response;
            }

            bool topUp = _transactionsService.TopUpBalance(topUpDetails.CustomerRef, topUpDetails.Amount);
            if(!topUp)
            {
                _response.IsSuccess = false;
                _response.ResponseCode = "01";
                _response.ErrorMessages.Add($"Error: Couldn't update balance for {topUpDetails.CustomerRef}. Contact Support");
                return _response;
            }
            _response.ResponseCode = "00";
            _response.Result = "Top up sucessful!";
            return _response;
        }
        [HttpPost]
        public ActionResult<APIResponse> PostTransaction(TransactionPostingDetails details)
        {
            
                if (details == null || details.Amount == 0)
                {
                    _response.IsSuccess = false;
                    _response.ResponseCode = "01";
                    _response.ErrorMessages.Add("Details provided are not supported for processing by System. Kindly review!");
                    return _response;
                }
                bool senderRefOk = _transactionsService.VerifyReference(details.SenderRef);
                bool receiverRefOk = _transactionsService.VerifyReference(details.ReceiverRef);
                if (!senderRefOk || !receiverRefOk)
                {
                    string forSenderRefOk = senderRefOk ? "Sender Reference Verified" : "Sender Reference does not exist";
                    string forReceiverRefOk = receiverRefOk ? "Receiver Reference Verified" : "Receiver Reference does not exist";
                    _response.IsSuccess=false;
                    _response.ResponseCode = "01";
                    _response.ErrorMessages.Add(forSenderRefOk);
                    _response.ErrorMessages.Add(forReceiverRefOk);
                    return _response;
                }

                bool isBalanceSufficient = _transactionsService.VerifyAccountBalance(details.SenderRef, details.Amount);
                if (!isBalanceSufficient)
                {
                    _response.IsSuccess = false;
                    _response.ResponseCode = "01";
                    _response.ErrorMessages.Add("Your balance is lower than the amount you are sending! Kindly top up your balance!");
                    return _response;
                }

                Transaction postedTransaction = _transactionsService.PostTransactionToLog(details);
                if (postedTransaction == null)
                {
                    _response.IsSuccess = false;
                    _response.ResponseCode="01";
                    _response.ErrorMessages.Add("An error occured while posting this transaction! Contact support");
                    return _response;
                }

                _response.ResponseCode = "00";
                _response.Result = postedTransaction;
                return _response;
        }
    }
}
