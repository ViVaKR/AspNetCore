
using System.Text.Json;

namespace WebApp;

public class HostedService(IHttpClientFactory _httpClientFactory) : IHostedService
{
    private readonly IHttpClientFactory httpClientFactory = _httpClientFactory;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var client = httpClientFactory.CreateClient();
        client.BaseAddress = new Uri("https://api.ipify.org");
        string ipAddress;
        string endpoint = "?format=json";
        try
        {
            using HttpResponseMessage response = await client.GetAsync(endpoint, cancellationToken);
            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            using var doc = JsonDocument.Parse(json);
            ipAddress = doc.RootElement.GetProperty("ip").GetString() ?? "0.0.0.0";
        }
        catch
        {
            ipAddress = "0.0.0.0";
        }
        Console.WriteLine($"\u001b[33mPublic IP Address: {ipAddress}\u001b[0m");
        await Console.Out.WriteLineAsync("\n\nHostedService is running.\n\n");
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
