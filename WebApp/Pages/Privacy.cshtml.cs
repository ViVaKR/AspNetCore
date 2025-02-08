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
        var taskBear = basicModel.GetAsync(urls[2], "1234");  // 첫 번째 태스크
        var taskGet = basicModel.GetAsync(urls[1]);           // 두 번째 태스크
        var taskFake = basicModel.GetAsync();                 // 세 번째 태스크

        // 실행 완료 순서와 관계없이 위에서 정의한 순서대로 결과가 배열에 저장됨
        var results = await Task.WhenAll(taskBear, taskGet, taskFake);

        // results[0]은 항상 taskBear의 결과
        ApiResponse += "== GET (Authentication Bearer Token) ==\n";
        ApiResponse += $"\nBearer Response:\n{results[0]}\n";

        // results[1]은 항상 taskGet의 결과
        ApiResponse += "\n== GET All ==\n";
        ApiResponse += $"\n{results[1]}\n";

        // results[2]는 항상 taskFake의 결과
        ApiResponse += "\n== Microsoft Fake HTTP GET All Request ==\n";
        ApiResponse += $"\n{results[2]}\n";
    }

}

