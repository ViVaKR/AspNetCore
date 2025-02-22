using GrpcGreeter.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();
var app = builder.Build();
app.MapGrpcService<GreeterService>();
app.MapGet("/", () => "Hello, World!");

app.Run();


var data = await File.ReadAllBytesAsync("C:/Users/USER/Desktop/GrpcGreeter/GrpcGreeter/GrpcGreeter.csproj");

