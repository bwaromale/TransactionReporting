using Grpc.Core;
using Grpc.Net.Client;
using System.Text.RegularExpressions;
using TransactionProcessing.Client;

namespace TransactionProcessing.Client.Services
{
    public class GreeterService : Greeter.GreeterBase
    {
        private readonly ILogger<GreeterService> _logger;
        public GreeterService(ILogger<GreeterService> logger)
        {
            _logger = logger;
        }

        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            using var channel = GrpcChannel.ForAddress("http://localhost:5240");
            //var client  = new TransactionProcessing.Server.GrpcClient(channel);
            return Task.FromResult(new HelloReply
            {
                Message = "Hello " + request.Name
            });
        }
    }
}

//using Grpc.Net.Client;
//using gRoom.gRPC.Messages;


//Console.WriteLine("Enter room name to register");
//var roomName = Console.ReadLine();

//var resp = client.RegisterToRoom(new RoomRegistrationRequest { RoomName = roomName });

//Console.WriteLine($"Room Id: {resp.RoomId}");
//Console.Read();