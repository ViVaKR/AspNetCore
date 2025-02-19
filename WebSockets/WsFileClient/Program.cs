using System.Net.WebSockets;
using System.Text;

//--> [ WebSocket Client, 클라이언트 ] <--//

var client = new ClientWebSocket();
string? name = "Client"; // 클라이언트 이름 설정
string? ext = "png"; // 클라이언트 확장 정보 설정

while (true)
{
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.Write("이름: ");
    Console.ResetColor();
    name = Console.ReadLine();  // 사용자로부터 이름 입력 받기
    if (string.IsNullOrEmpty(name)) continue;  // 이름이 입력되면 반복문 종료

    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.Write("확장자: ");
    Console.ResetColor();
    ext = Console.ReadLine();  // 사용자로부터 확장자 입력 받기
    if (!string.IsNullOrEmpty(ext)) break;  // 확장자가 입력되면 반복문 종료
}

//? 서버연결 URI 설정
Uri serverUri = new($"wss://localhost:29018/wss?name={name}&ext={ext}"); // 서버의 WebSocket URI

// 서버연결
Console.WriteLine($"Connecting to server at {serverUri}...");
await client.ConnectAsync(serverUri, CancellationToken.None);
Console.WriteLine("Connected to server.");

// 파일을 보내는 작업을 별도의 Task로 처리 (비동기적으로 실행)
var sendFileTask = Task.Run(async () =>
{
    var buffer = new byte[1024 * 16];  // 8KB 크기의 버퍼 설정

    while (true)
    {
        //? 메뉴선택 : 메시지 전송, 파일 전송, 종료
        // Console.Write("(선택) \u001b[31m`\n1. Message`\u001b[0m, \u001b[31m`\n2. Sendfile`\u001b[0m, \u001b[31m`\n3. Exit`\u001b[0m >> ");

        SetMenu();

        //? 사용자로부터 메뉴 선택 입력 받기
        var message = Console.ReadLine();

        switch (message?.ToLower())
        {
            // 문의에 대한 답변 파트
            case "sendfile": //--> 파일 전송 요청
                Console.Write("전송할 파일 경로: ");
                string? filePath = Console.ReadLine();

                if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
                {
                    Console.WriteLine("파일을 찾을 수 없습니다.");
                    continue;
                }

                try
                {
                    using var fileStream = File.OpenRead(filePath);
                    var buf = new byte[16 * 1024]; // 16KB 버퍼
                    int bytesRead;
                    long totalBytesSent = 0;

                    var fi = new FileInfo(filePath);
                    var fileSize = fi.Length;

                    Console.WriteLine($"파일전송개시: {filePath} (Size: {fileSize:N0} bytes)");

                    while ((bytesRead = await fileStream.ReadAsync(buf)) > 0)
                    {
                        var dataToSend = new ArraySegment<byte>(buf, 0, bytesRead);

                        // 파일 전송
                        await client.SendAsync(dataToSend,
                            WebSocketMessageType.Binary,
                            //? (문의에 대한 답) --> 마지막 청크인 경우에만 true
                            totalBytesSent + bytesRead >= fileSize,
                            CancellationToken.None);

                        totalBytesSent += bytesRead;
                        // 데모용 딜레이
                        await Task.Delay(10).ContinueWith(x =>
                        {
                            Console.Write($"\rSending progress: {totalBytesSent * 100 / fileSize:N0}%");
                        });
                    }

                    Console.WriteLine($"\nFile '{filePath}' sent successfully. Total bytes sent: {totalBytesSent:N0}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\n전송 오류: {ex.Message}");
                }
                break;

            case "message": //--> 텍스트 메시지 전송 요청
                {
                    Console.Write("전송할 메시지 >> ");
                    var msg = Console.ReadLine();
                    if (string.IsNullOrEmpty(msg)) continue;
                    var bytes = Encoding.UTF8.GetBytes(msg);
                    await client.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
                }
                break;

            case "exit": //--> 연결 종료 요청
                if (client.State == WebSocketState.Open)
                    await client.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client closed", CancellationToken.None);  // 서버에 정상 종료 요청
                break;

            default:
                continue;

        }
    }
});

//* 수신을 위한 Task (비동기적으로 수신 대기)
var receiveTask = Task.Run(async () =>
{
    var buffer = new byte[8192];  // 8KB 크기의 버퍼 설정
    using MemoryStream fileStream = new(); // 수신한 파일 데이터를 기록할 메모리 스트림 생성

    while (client.State == WebSocketState.Open)  // 클라이언트가 열려있는 동안 반복
    {
        try
        {
            var result = await client.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);  // 서버로부터 데이터 수신

            // 서버가 연결을 종료하면 종료 메시지를 출력하고 반복문을 종료
            if (result.MessageType == WebSocketMessageType.Close)
            {
                Console.WriteLine("Server has closed the connection.");
                break;  // 서버 연결 종료 시 종료
            }

            // 서버가 바이너리 데이터를 보내는 경우 (파일 데이터)
            if (result.MessageType == WebSocketMessageType.Binary)
            {
                Console.WriteLine("Receiving file...");  // 파일 수신 시작 메시지 출력
                await ReceiveFileChunk(fileStream, buffer, result.Count);  // 파일 청크를 받아 메모리 스트림에 기록
            }
            // 서버가 텍스트 메시지를 보내는 경우
            else if (result.MessageType == WebSocketMessageType.Text)
            {
                var message = Encoding.UTF8.GetString(buffer, 0, result.Count);  // 텍스트 메시지 디코딩
                Console.Write($"\n(수신메시지 {DateTime.Now}) : \u001b[31m{message}\u001b[0m\n");  // 수신한 메시지 출력
                SetMenu();
            }
        }
        catch (WebSocketException ex) when (ex.WebSocketErrorCode == WebSocketError.ConnectionClosedPrematurely)
        {
            Console.WriteLine("서버와의 연결이 종료되었습니다.");
            break;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"수신 중 오류 발생: {ex.Message}");
            break;
        }
    }
});

//* 클라이언트가 서버와의 연결을 종료할 때까지 대기
try
{
    await Task.WhenAny(sendFileTask, receiveTask);
}
catch (Exception ex)
{
    Console.WriteLine($"작업 실행 중 오류 발생: {ex.Message}");
}

//* 서버와의 연결이 열려 있으면 정상적으로 연결 종료 시도
try
{
    if (client.State == WebSocketState.Open)
    {
        await client.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "Client closed", CancellationToken.None);
        Console.WriteLine("WebSocket 연결이 정상적으로 종료되었습니다.");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"연결 종료 중 오류 발생: {ex.Message}");
}

//* 모든 비동기 작업이 종료될 때까지 대기
try
{
    await Task.WhenAll(sendFileTask, receiveTask);
}
catch
{
    // 종료 과정에서 발생하는 예외는 무시
}

Console.WriteLine("프로그램이 종료되었습니다.");

// 파일 수신 메서드 (메모리 스트림에 파일 청크 기록)
static async Task ReceiveFileChunk(MemoryStream fileStream, byte[] buffer, int byteCount)
{
    await fileStream.WriteAsync(buffer.AsMemory(0, byteCount));  // 비동기적으로 메모리 스트림에 청크 기록
    Console.WriteLine($"Received {byteCount} bytes, writing to file...");  // 수신한 바이트 수와 함께 파일 기록 상태 출력
}

static void SetMenu()
{
    Console.WriteLine("메뉴선택: ");
    Console.WriteLine("1. Message");
    Console.WriteLine("2. Sendfile");
    Console.WriteLine("3. Exit");
    Console.Write(">> ");
}
