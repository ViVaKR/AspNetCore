using System.Security.Claims;
using GameStore.Api.Data;
using GameStore.Api.Endpoints;

Dictionary<string, List<string>> gamesMap = new()
{
    {"player1", ["Street Fighter II", "Minecraft"]},
    {"player2", ["Forza Horizon 5", "Final Fantasy XIV", "FIFA 23"]}
};

Dictionary<string, List<string>> subscriptionMap = new()
{
    {"silver", ["Street Fighter II", "Minecraft"]},
    {"gold", ["Forza Horizon 5", "Minecraft", "Forza Horizon 5", "Final Fantasy XIV", "FIFA 23"]}
};

var builder = WebApplication.CreateBuilder(args);

// Jwt
builder.Services.AddAuthentication().AddJwtBearer();

/*
    !(create token examples, cli in terminal)
    --> $ dotnet user-jwts create
    --> $ dotnet user-jwts create --role "admin"
    --> $ dotnet user-jwts create --role "player"
    --> $ dotnet user-jwts create --role "player" -n player1
    --> $ dotnet user-jwts create --role "player" -n player1 --claim "subscription=gold"

    --> $ dotnet user-jwts print <ID>
    --> viewer --> browser --> url (https://jwt.ms)--> copy & paste (token)
 */

builder.Services.AddAuthorization();

string? connString = builder.Configuration.GetConnectionString("GameStore");

builder.Services.AddSqlite<GameStoreContext>(connString);
//? builder.Services.AddScoped<GameStoreContext>

var app = builder.Build();

app.Logger.LogInformation(5, "The database is ready!");

//? token tester
app.MapGet("/playergames", () => gamesMap).RequireAuthorization(policy =>
{
    // 엑세스 제어
    policy.RequireRole("admin");
});

app.MapGet("/mygames", (ClaimsPrincipal user) =>
{
    var hasClaim = user.HasClaim(claim => claim.Type == "subscription");

    if (hasClaim)
    {
        var subs = user.FindFirstValue("subscription") ?? throw new Exception("Claim has no value!");

        return Results.Ok(subscriptionMap[subs]);
    }

    ArgumentNullException.ThrowIfNull(user.Identity?.Name);
    var username = user.Identity.Name;

    if (!gamesMap.TryGetValue(username, out List<string>? value))
        return Results.Empty;

    return Results.Ok(value);

}).RequireAuthorization(x =>
{
    x.RequireRole("player");
});

app.MapGamesEndpoints();

app.MapGenresEndpoints();

await app.MigrateDbAsync();

app.Run();

/* ====== Note ==============================================================

! [ Authorization ]
1. nuget package -> Microsoft.AspNetCore.Authentication.JwtBearer
`dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer`

- `Jwt` -> Json Web Token

--> Authentication : (신원확인)
    * 로그인 등으로 인증하는 절차, 사람을 확인하는 절차
    *- 지식기반 : 패스워드, 이름, 주민번호, 사번 등..
    *- 소유기반 : 인증서, SMS 인증, OTP
    *- 속성기반 : 지문, 홍채, 정맥, 얼굴 등등 ..
        ** One Factor, Two Factor,

--> Authorization : (엑세스권한 확인)
    * 권한 부여, 원하는 곳에 접근할 수 있고 정보를 취득할 수 있도록 허용하는 절차

--> Access Control : 접근 제어

! [ REST Client ]

! [ Nuget packages]
1. MiniamlApis.Extensions
2. Microsoft.EntityFrameworkCore.Sqlite
3. ( Migration tool ) nuget search -> `dotnet-ef`
    ?-> $ dotnet add package Microsoft.EntityFrameworkCore.Sqlite
    ?-> $ dotnet tool install --global dotnet-ef
    ?-> $ dotnet add package Microsoft.EntityFrameworkCore.Design
    ?-> $ dotnet add package Microsoft.EntityFrameworkCore.Tools
        Add-Migration
        Bundle-Migration
        Drop-Database
        Get-DbContext
        Get-Migration
        Optimize-DbContext
        Remove-Migration
        Scaffold-DbContext
        Script-Migration
        Update-Database

4. Migrations & Update
    --> $ dotnet ef migrations add InitialCreate --output-dir Data/Migrations
    --> $ dotnet ef database update

! [ Configuration System ]
1. `IConfiguration` interface --> (appsettings.json, 'Connection string') --> REST API --> Database
    -> Command Line Args
    -> Environment Variables
    -> User Secrets
    -> Cloud Sources

![ Dependency Injection ]

? What is a Dependency?

--> MyService --->(LogThis("foo")---> MyLogger

public MyService()
{
    var logger = new MyLogger();
    logger.LogThis("I am Ready");
}

public MyService()
{
    var writter = new MyFileWritter("output.log");
    var logger = new MyLogger(writter);
    logger.LogThis("I am Ready");
}

 ** Problems **
 - MyService is tightly coupled to the Logger dependency.
 - Any changes to MyLogger require changes to MyService.
 - MyService needs to know how to construct and configure the MyLogger dependency.
 - It's hard to test MyService since the MyLogger dependency cannot be mocked or stubbed.
 - My Service는 Logger 종속성과 밀접하게 연결되어 있습니다.
 - MyLogger를 변경하려면 MyService를 변경해야 합니다.
 - MyService는 MyLogger 종속성을 구성하고 구성하는 방법을 알아야 합니다.
 - MyLogger 의존성을 조롱하거나 찌그러뜨릴 수 없기 때문에 MyService를 테스트하기가 어렵습니다.

! [ What is Dependency Injection? ]

--> MyService --->(LogThis("foo")---> MyLogger

? | MyLogger, Another Dependency --> (Register) --> Service Container (IServiceProvider) --> Resolve, construct and inject dependencies --> MyService <-- HTTP Request |

public MyService(MyLogger logger)
{
    logger.LogThis("I am Ready");
}

? Benefits
- MyService won't be affected by changes to its dependencies.
- MyService doesn't need to know how to construct or configure its dependencies.
- Dependencies can also be injected as parameters to minimal API endpoints
- Opens the door to using Dependency Inversion.

! [ Using Dependency Inversion ]

My Service는 Logger 종속성과 밀접하게 연결되어 있습니다.
MyLogger를 변경하려면 MyService를 변경해야 합니다.
MyService는 MyLogger 종속성을 구성하고 구성하는 방법을 알아야 합니다.
MyLogger 의존성을 조롱하거나 찌그러뜨릴 수 없기 때문에 MyService를 테스트하기가 어렵습니다.

- My Service는 종속성 변경에 영향을 받지 않습니다.
- My Service는 종속성을 구성하거나 구성하는 방법을 알 필요가 없습니다.
- 종속성은 최소 API 엔드포인트에 매개변수로 주입할 수도 있습니다.
- Dependency Inversion 사용의 문을 엽니다.

의존성 반전 원리.

- 코드는 구체적인 구현이 아닌 추상화에 의존해야 합니다.

public MyService(ILogger logger)
{
    logger.LogThis("I am Ready!");
}

! AddTransient
MyLogger ---> (AddTransient<MyLooger>()) ---> IServiveProvider ---> MyLogger (Resolve, Construct, Inject) ---> MyService (use MyLogger) <--- HTTP Request

! AddScoped

! AddSingleton : 응용프로그램이 종료될 때 까지. (acros the application lifetime)

! Performing asynchronous work
    >- 단순화된 코드: 비동기 코드는 작업 개체와 비동기 및 대기 키워드를 통해 쓰기 쉽습니다.
    >- 향상된 확장성: 애플리케이션이 더 많은 요청과 사용자를 동시에 처리할 수 있습니다.
    >- 향상된 성능: 호출자간의 차단을 피하고 다른 작업을 수행할 수 있습니다.

! Logging

Application Code --> Logger (ILogger) <--(Register) --- Application Startup

1. LogInformation ("Operation completed")
2. LogWarning ("Something is not OK")
3. LogError ("Something bad happened")

Logging Providers ---> Console, Event Log (Windows Event Viewer), Event Source (Perfview), Seq, App Service

! Token-Based Authentication

- Authentication
- Authorization

- Included in the token
    1. How to verify it
    2. Where it came from
    3. Whre it can be used
    4. Who is authorized
    5. What can be done with it
    6. Much more

User ---> Request authorization ---> Authorization Server
User <------------------------------ (Access token)

User ---> REST API request + Access token ---> Games API
User <---------------------------------------- (Response)

! Decoded Token example (https://jwt.ms)
Header - Body - Signature
{
  "alg": "HS256",
  "typ": "JWT"
}.{
  "unique_name": "vivakr",
  "sub": "vivakr",
  "jti": "428c9787",
  "aud": [
    "http://localhost:12656",
    "https://localhost:44377",
    "http://localhost:5134",
    "https://localhost:7257"
  ],
  "nbf": 1710233487,
  "exp": 1718182287,
  "iat": 1710233487,
  "iss": "dotnet-user-jwts"
}.[Signature]

*/
