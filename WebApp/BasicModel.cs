using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp;

//--> PageModel을 상속받으며 생성자 종속성(DI) 주입 예시 (C# 9.0 문법 사용)
public class BasicModel(IHttpClientFactory httpClientFactory) : PageModel
{
    // IHttpClientFactory를 주입받은 인스턴스를 읽기 전용 필드에 저장하여 HttpClient 생성에 사용
    private readonly IHttpClientFactory httpClientFactory = httpClientFactory;

    // 기본 API URL을 나타내는 Uri 객체를 읽기 전용 필드로 설정 (JsonPlaceholder API)
    private readonly Uri url1 = new("https://jsonplaceholder.typicode.com");

    //* HTTP 요청을 보내고 응답 콘텐츠를 문자열로 반환하는 비동기 메서드
    private async Task<string> SendRequestAsync(HttpRequestMessage request)
    {
        // IHttpClientFactory를 사용하여 HttpClient 인스턴스를 생성
        var client = httpClientFactory.CreateClient();

        // HttpClient를 사용해 요청을 비동기적으로 전송하고 응답을 받아옴
        using HttpResponseMessage response = await client.SendAsync(request);

        // 응답 상태가 성공(200번대)이 아니면 예외를 발생시키며, 성공 시 요청 정보를 콘솔에 출력
        response.EnsureSuccessStatusCode().WriteRequestToConsole();

        // 콘솔에 응답 상태 코드와 헤더 정보를 ANSI 색상 코드를 사용하여 출력
        Console.WriteLine($"\u001b[33mResponse Status => {(int)response.StatusCode}\nHeaders =>\n{response.Headers}\n\u001b[0m");

        // 응답 본문을 문자열로 읽어 반환
        return await response.Content.ReadAsStringAsync();
    }

    //* 전달된 endpoint URL로 GET 요청을 보내고 응답 본문을 반환하는 비동기 메서드
    public async Task<string> GetAsync(string endpoint)
    {
        // GET 요청을 위한 HttpRequestMessage 객체 생성 (HTTP 메서드는 GET, URL은 endpoint)
        var request = new HttpRequestMessage(HttpMethod.Get, endpoint);

        // SendRequestAsync 메서드를 사용해 요청을 전송하고 응답 결과를 반환
        return await SendRequestAsync(request);
    }

    //* (문의에 대한 답변) URL과 Bearer 토큰을 이용한 인증된 GET 요청을 보내고 응답 본문을 반환하는 비동기 메서드
    //* 사전에 로그인 등으로 발급받은 Bearer Token 을 전달하여 인증된 요청을 보낼 수 있음을 시뮬레이뮬
    public async Task<string> GetAsync(string url, string bearerToken)
    {
        // GET 요청을 위한 HttpRequestMessage 객체 생성 (HTTP 메서드는 GET, URL은 전달된 값)
        var request = new HttpRequestMessage(HttpMethod.Get, url);

        // 응답 데이터의 미디어 타입을 제한 없이 수신하기 위해 Accept 헤더에 "*/*" 추가
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));

        // 요청 헤더에 Authorization 정보를 추가하여 Bearer 토큰으로 인증 설정 : "Bearer {bearerToken}"
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

        // SendRequestAsync 메서드를 호출하여 요청을 전송하고 응답 결과를 반환
        return await SendRequestAsync(request);
    }

    //* 기본 URL(url1)을 사용하여 "todos" 리소스에 GET 요청을 보내고 응답 본문을 반환하는 비동기 메서드
    public async Task<string> GetAsync()
    {
        // IHttpClientFactory를 사용하여 HttpClient 인스턴스를 생성
        var client = httpClientFactory.CreateClient();

        // HttpClient의 기본 주소(BaseAddress)를 설정 (url1)
        client.BaseAddress = url1;

        // "todos" 엔드포인트로 GET 요청을 보내고 응답을 받아옴
        using HttpResponseMessage response = await client.GetAsync("todos");

        // 응답 상태가 성공(200번대)가 아니면 예외 발생, 성공시 요청 정보를 콘솔에 출력
        response.EnsureSuccessStatusCode().WriteRequestToConsole();

        // 콘솔에 응답 상태와 헤더를 출력 (ANSI 색상 코드 사용)
        Console.WriteLine($"\u001b[33mResponse Status => {(int)response.StatusCode}\nHeaders =>\n{response.Headers}\n\u001b[0m");

        // 응답 본문을 문자열로 읽어 변수에 저장
        var jsonResponse = await response.Content.ReadAsStringAsync();

        // 읽은 문자열 응답을 반환
        return jsonResponse;
    }
}
