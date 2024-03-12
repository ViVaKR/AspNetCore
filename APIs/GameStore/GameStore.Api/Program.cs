using GameStore.Api.Data;
using GameStore.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);
var connString = builder.Configuration.GetConnectionString("GameStore");

// Dependency Injection
builder.Services.AddSqlite<GameStoreContext>(connString);
//? builder.Services.AddScoped<GameStoreContext>

var app = builder.Build();

app.MapGamesEndpoints();
app.MapGenresEndpoints();

// 데이터 베이스 자동 마이그레이션.
await app.MigrateDbAsync();

app.Run();

/*
! [ Nuget packages]
1. MiniamlApis.Extensions
2. Microsoft.EntityFrameworkCore.Sqlite
3. ( Migration tool ) nuget search -> `dotnet-ef`
    ?-> install -> $ dotnet add package Microsoft.EntityFrameworkCore.Sqlite
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
    -> $
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
*/
