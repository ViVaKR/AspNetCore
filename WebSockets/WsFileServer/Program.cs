using System.Net.WebSockets;
using System.Text;
using System.Net;

//--> [ WebSocket Server, 서버 ] <--//

var builder = WebApplication.CreateBuilder(args);

//? Server HTTPS URL, Port (== wss://localhost:29018)
builder.WebHost.UseUrls("https://localhost:29018");

var app = builder.Build();

//? 웹소켓 미들웨어를 사용하도록 설정
app.UseWebSockets();

//? 연결된 WebSocket 클라이언트들을 저장할 리스트
var connections = new List<WebSocket>();

//? "/wss" 경로에 대해 WebSocket 요청을 처리하는 엔드포인트 설정
app.Map("/wss", async context =>
{
    //* WebSocket이 아닌 일반 HTTP 요청이 들어오면 400 오류 반환
    if (!context.WebSockets.IsWebSocketRequest)
    {
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        return;
    }
    var tempName = context.Request.Query["name"]!;  // 클라이언트의 이름을 쿼리 파라미터에서 가져옵니다.
    var ext = context.Request.Query["ext"]!;  // 클라이언트의 확장자 정보를 쿼리 파라미터에서 가져옵니다.

    var name = $"{tempName}_{DateTime.Now.Ticks}"; // 클라이언트 이름에 현재 시간을 추가하여 고유한 이름으로 설정

    Console.WriteLine($"(웹소켓 연결 요청 {DateTime.Now})\n- 이름: {name}\n- 파일 확장자: {ext}");  // 클라이언트 이름과 확장자 정보 출력

    // WebSocket 연결을 승인하고 클라이언트를 연결된 리스트에 추가
    using var wss = await context.WebSockets.AcceptWebSocketAsync();

    connections.Add(wss);

    // 클라이언트가 방에 참가했음을 방송
    await Broadcast($"{tempName} 님이 입장하셨습니다. 환영합니다!\n총 ({connections.Count})명이 방에 참가하였습니다.");

    // 파일을 수신하는 메서드 호출, 수신된 데이터는 청크로 처리됨
    await ReceiveFile(wss, async (result, buffer) =>
    {
        switch (result.MessageType)
        {
            case WebSocketMessageType.Binary: //--> 수신된 메시지 유형이 바이너리 데이터(파일)일 경우
                {
                    string? filePath = $"{name}.{ext}";  // 파일을 저장할 경로 지정
                    await ReceiveFileChunk(buffer, result.Count, filePath ?? $"Received_{DateTime.Now.Ticks}");  // 수신된 청크를 파일에 저장
                }
                break;
            case WebSocketMessageType.Text: //--> 수신된 메시지 유형이 텍스트인 경우
                {
                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);  // 수신된 텍스트 메시지 디코딩
                    await Broadcast($"{tempName}: {message}");  // 수신된 메시지를 모든 클라이언트에게 방송
                }
                break;
            case WebSocketMessageType.Close: //--> 수신된 메시지가 닫힘 요청인 경우
                {
                    connections.Remove(wss);  // 연결 종료 시 연결 리스트에서 제거
                    await Broadcast($"{tempName} Left the room"); // 클라이언트가 방을 떠났음을 방송
                    await Broadcast($"Total connections: {connections.Count}"); // 현재 연결된 클라이언트 수를 방송
                    var status = result.CloseStatus ?? WebSocketCloseStatus.NormalClosure;// 연결 종료 상태를 설정하고 클라이언트와의 연결을 닫음
                    await wss.CloseAsync(status, result.CloseStatusDescription, CancellationToken.None);
                }
                break;
        }
    });
});

// 파일을 수신하고 처리하는 메서드
async Task ReceiveFile(WebSocket socket, Func<WebSocketReceiveResult, byte[], Task> handleMessage)
{
    var buffer = new byte[1024 * 16];  // 버퍼크기 16KB
    long totalBytes = 0;  // 수신된 전체 바이트 수를 저장할 변수
    while (socket.State == WebSocketState.Open)
    {
        try
        {
            var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            if (result.MessageType == WebSocketMessageType.Close) break;
            await handleMessage(result, buffer);
            totalBytes += result.Count;
        }
        catch (WebSocketException ex)
        {
            Console.WriteLine($"WebSocketException: {ex.Message}");
            await Broadcast($"WebSocketException: {ex.Message}");
            break;
        }
    }
}

// 수신된 파일 청크를 메모리 스트림에 저장하는 메서드
async Task ReceiveFileChunk(byte[] buffer, int byteCount, string filePath)
{
    try
    {
        using FileStream fs = new(filePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
        await fs.WriteAsync(buffer.AsMemory(0, byteCount));
        await fs.FlushAsync();

        Console.WriteLine($"Received chunk: {byteCount:N0} bytes for {filePath}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error writing file chunk: {ex.Message}");
        await Broadcast($"Error writing file chunk: {ex.Message}");
    }
}

// 연결된 모든 클라이언트에게 메시지를 방송하는 메서드
async Task Broadcast(string message)
{
    Console.WriteLine($"Broadcasting: {message}");  // 방송 메시지 출력
    var bytes = Encoding.UTF8.GetBytes(message);  // 메시지를 바이트 배열로 변환
    // 모든 WebSocket 클라이언트에게 메시지를 전송
    foreach (var socket in connections)
    {
        if (socket.State == WebSocketState.Open)  // 클라이언트가 연결 상태인 경우
        {
            var arraySegment = new ArraySegment<byte>(bytes);  // 바이트 배열을 WebSocket 메시지 형태로 변환
            await socket.SendAsync(arraySegment, WebSocketMessageType.Text, true, CancellationToken.None);  // 메시지 전송
        }
    }
}

// 애플리케이션 실행
await app.RunAsync();
