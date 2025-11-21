using Microsoft.IdentityModel.Protocols.OpenIdConnect;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    ApplicationName = typeof(Program).Assembly.FullName,
    ContentRootPath = Directory.GetCurrentDirectory(),
    EnvironmentName = Environments.Staging,
    WebRootPath = "wwwroot"
    
});

builder.Services.AddControllers();

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapControllers();
app.UseDefaultFiles();
app.UseAuthorization();
app.UseStaticFiles();
app.Run();

/*
 ** 미들웨어 **
 
 UseAuthentication
 UseAuthorization
 UseCors
 UseExceptionHandler
 UseForwardedHeaders
 UseHttpsRedirection
 UseHsts
 UseHttpLogging
 UseW3Clogging
 UseResponseCaching
 UseResponseCompression
 UseSession
 UseStaticFiles
 UseFileServer
 UseWebSockets
 
 */

/*

 $ dotnet run --urls="http://localhost:4000"
 app.Urls.Add("http://localhost:3000");
 app.Urls.Add("http://localhost:4000");
 app.Urls.Add("http://*.3000");
 app.Urls.Add("http://+.3000");
 app.Urls.Add("http://0.0.0.0:300")
 ASPNETCORE_URLS=http://*:3000;https://+:5000;http://0.0.0.0:5005
 ASPNETCORE_HTTP_PORTS=3000;5005
 ASPNETCORE_HTTPS_PORTS=5000
 var port = Environment.GetEnvironmentVariable("PORT");
 Console.BackgroundColor = ConsoleColor.Red;
 Console.ForegroundColor = ConsoleColor.DarkYellow;
 Console.ResetColor();
 
*/

