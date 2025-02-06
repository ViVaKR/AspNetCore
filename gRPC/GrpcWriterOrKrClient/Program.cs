// See https://aka.ms/new-console-template for more information
using Grpc.Net.Client;
using GrpcWriterOrKrClient;
// using GrpcWriterOrKr;

using var channel = GrpcChannel.ForAddress("https://localhost:7076");
var client = new Greeter.GreeterClient(channel);
var reply = await client.SayHelloAsync(new HelloRequest { Name = "GreeterClient" });

Console.WriteLine("Greeting: " + reply.Message);
Console.ReadKey();
