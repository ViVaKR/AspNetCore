using ApiWriterOrKR.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc();

var app = builder.Build();

// Confgure the HTTP request pipeline.
app.MapGrpcService<GreeterService>();
// Configure the HTTP request pipeline.
app.MapGrpcService<GreeterService>();
app.MapGet("/", () => "GRPC Server is running");

app.Run();
