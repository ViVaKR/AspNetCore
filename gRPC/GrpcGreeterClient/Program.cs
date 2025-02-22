using Grpc.Net.Client;
using GrpcGreeterClient;
using GrpcGreeterClient.Models;


using var channel = GrpcChannel.ForAddress("https://localhost:7254");

var client = new Greeter.GreeterClient(channel);

var reply = await client.SayHelloAsync(new HelloRequest
{ Name = "안녕하세요 반값습니다." });

await Console.Out.WriteLineAsync($"Greeting: {reply.Message}");
