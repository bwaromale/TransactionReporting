using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using TransactionProcessing.Server.Protos;
using TransactionReportingAPI.Data;
using TransactionReportingAPI.Models;
using TransactionReportingAPI.Models.DTOs;
using Transaction = TransactionReportingAPI.Models.Transaction;

namespace TransactionReportingAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly TransactionProcessingContext _db;
        private readonly SpoolPendingTransactions.SpoolPendingTransactionsClient _grpcClient;
        protected APIResponse _response = new APIResponse();
        public TransactionsController(TransactionProcessingContext db)
        {
             _db = db;
            GrpcChannel channel = GrpcChannel.ForAddress("https://localhost:7105");
            _grpcClient = new SpoolPendingTransactions.SpoolPendingTransactionsClient(channel);
        }
        [HttpGet]
        public async Task<ActionResult<APIResponse>> GetPendingTransactions()
        {
            List<Transaction> transactions = new List<Transaction>();
            var input = new PendingTransactionsRequest();
            var respFrmGrpc = _grpcClient.FetchTransactions(input);
            
            await foreach (var streamedTransaction in respFrmGrpc.ResponseStream.ReadAllAsync())
            {
                Transaction transaction = new Transaction();
                transaction.TransactionId = streamedTransaction.TransactionId;
                transaction.SenderRef = streamedTransaction.SenderRef;
                transaction.ReceiverRef = streamedTransaction.ReceiverRef;
                transaction.Amount = streamedTransaction.Amount;
                transaction.CreateDate = streamedTransaction.CreateDate.ToDateTime();
                transactions.Add(transaction);
            }
            
            _response.StatusCode = HttpStatusCode.OK;
            _response.Result = transactions;
            return _response;
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
