using Grpc.Core;
using Grpc.Net.Client;
using GrpcService1;

namespace Helmut.Controls.Features;


internal interface IOperationsGrpcService
{
    Task<HelloReply> ExecuteAsync();
    IAsyncEnumerable<HelloReply> Execute2Async();
}

internal sealed class OperationsGrpcService : IOperationsGrpcService
{
    public async IAsyncEnumerable<HelloReply> Execute2Async()
    {
        using var channel = GrpcChannel.ForAddress("https://localhost:7262");
        var client = new Greeter.GreeterClient(channel);

        await foreach (var reply in client.StreamHello(new HelloRequest { Name = "GreeterClient" }).ResponseStream.ReadAllAsync())
        {
         //   await Task.Delay(500);
            yield return reply;
        }
    }

    public async Task<HelloReply> ExecuteAsync()
    {
        using var channel = GrpcChannel.ForAddress("https://localhost:7262");
        var client = new Greeter.GreeterClient(channel);
        var reply = await client.SayHelloAsync(new HelloRequest { Name = "GreeterClient" });
        return reply;
    }
}