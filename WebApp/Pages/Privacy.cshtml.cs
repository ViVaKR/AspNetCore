using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages;

public class PrivacyModel(BasicModel basicModel) : PageModel
{
    private readonly BasicModel basicModel = basicModel;
    private readonly Dictionary<int, string> urls = new()
    {
        { 1, "https://httpbin.org/get" },
        { 2, "https://httpbin.org/bearer" },
    };

    public string ApiResponse { get; set; } = string.Empty;

    public async Task OnGetAsync()
    {
        var taskBear = basicModel.GetAsync(urls[2], "1234");
        var taskGet = basicModel.GetAsync(urls[1]);
        var taskFake = basicModel.GetAsync();
        await Task.WhenAll(taskBear, taskGet, taskFake);

        ApiResponse += "== GET (Authentication Bearer Token) ==\n";
        ApiResponse += $"\nBearer Response:\n{await taskBear}\n";

        ApiResponse += "\n== GET All ==\n";
        ApiResponse += $"\n{await taskGet}\n";

        ApiResponse += "\n== Microsoft Fake HTTP GET All Request ==\n";
        ApiResponse += $"\n{await taskFake}\n";
    }

}

