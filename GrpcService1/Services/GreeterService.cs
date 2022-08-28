using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using GrpcGreeterClient;
using GrpcService1;
using System;

namespace GrpcService1.Services
{
    public class GreeterService : Greeter.GreeterBase
    {
        private readonly ILogger<GreeterService> _logger;
        private static readonly Random _random = new();
        public GreeterService(ILogger<GreeterService> logger)
        {
            _logger = logger;
        }

        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            return Task.FromResult(new HelloReply
            {
                Message = "Hello " + request.Name
            });
        }

        public override async Task StreamHello(HelloRequest request, IServerStreamWriter<HelloReply> responseStream, ServerCallContext context)
        {
            var iterator = 0;
            while (true)
            {
                var waited = TimeSpan.FromSeconds(_random.Next(1, 5));
                await Task.Delay(waited);

                var reply = new HelloReply()
                {
                    Message = "Hello " + request.Name,
                    Count = iterator++,
                    DateTimeStamp = Timestamp.FromDateTimeOffset(DateTimeOffset.UtcNow),
                    Duration = Duration.FromTimeSpan(waited),

                };

                if (context.CancellationToken.IsCancellationRequested) break;

                await responseStream.WriteAsync(reply);
            }
        }
    }
}

// | python -m json.tool