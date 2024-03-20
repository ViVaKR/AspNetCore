using Microsoft.AspNetCore.ResponseCompression; //--> (1)
using VivChat.Hubs; //--> (2) ChatHub Namespace
using VivChat.Components;

var builder = WebApplication.CreateBuilder(args);

//--> (3) Add Compression MiddleWare
builder.Services.AddResponseCompression(x => x.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(["application/octet-stream"]));

// Add services to the container.
builder.Services.AddRazorComponents().AddInteractiveServerComponents();

var app = builder.Build();

//--> (4) Use Compression MiddleWare
app.UseResponseCompression();

//--> (5) EndPoint
app.MapHub<ChatHub>("/chathub");

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.Run();

//--> (final) edit -> Home.razor
