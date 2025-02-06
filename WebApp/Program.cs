using WebApp;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddHttpClient();
builder.Services.AddTransient<BasicModel>();
builder.Services.AddHostedService<HostedService>();

builder.WebHost.UseUrls("https://localhost:54912");

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

using var cts = new CancellationTokenSource();
var token = cts.Token;

// app.Run();
await app.RunAsync(token);
