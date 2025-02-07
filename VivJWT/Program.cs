using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.WithOrigins("https://localhost:51341") // 허용할 Origin을 지정합니다. 필요에 따라 수정하세요.
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

builder.WebHost.UseUrls("https://localhost:51341");

builder.Services.AddAuthentication("LocalAuthIssuer").AddJwtBearer("LocalAuthIssuer", options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Authentication:Schemes:LocalAuthIssuer:ValidIssuer"],
        ValidAudiences = builder.Configuration.GetSection("Authentication:Schemes:LocalAuthIssuer:ValidAudiences").Get<string[]>(),
        IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(builder.Configuration["Authentication:Schemes:LocalAuthIssuer:SecretKey"]!))
    };
    Console.WriteLine($"IssuerSigningKey: {options.TokenValidationParameters.IssuerSigningKey}");
});


// 권한 부여 서비스 등록
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => "Hello World!");

// hello 엔드포인트에 접근하기 위해 사용자가 인증되어야 함을 명시 함.
// 인증된 사용자만 이 엔드 포인트에 접근할 수 있음.
// RequireAuthorization() 메서드는 인증과 권한 부여 모두를 포함하는 개념
// 즉, 사용자가 인증되었는지 확이하고 필요한 권한이 있는지도 확인함.
app.MapGet("/hello", () => "Hello World!\n").RequireAuthorization();

app.Run();



/*

--> Authentication (인증) : 로그인 또는 토큰 기반 인증을 통해 자신의 신원을 증명함.
목적 : 사죵자가 주장하는 신원이 실제로 그 사람의 신원인지 확인하는 것.
방법 : 토큰 기반 인증, 사용자 이름과 비밀번호를 이용한 인증, 소셜 로그인, 생테인식
예시 : 로그인 화면에서 사용자 이름과 비밀번호를 입력하는 과정
    1. 토큰 발급
        - JWT(Json Web Token) : Header, Payload, Signature로 구성
        - Header : 토큰의 타입과 해싱 알고리즘 정보
        - Payload : 클레임(claim) 정보
        - Signature : Header와 Payload를 Base64로 인코딩한 값에 Secret Key를 이용해 해싱한 값
    2. 토큰 검증
        - 토큰의 유효성을 검증
    3. 토큰 갱신
        - 토큰의 만료 시간이 지나면 갱신
    4. 토큰 폐기
        - 토큰을 폐기하면 해당 토큰을 사용할 수 없음.

--> Authorization (권한 부여)
목적 : 인증된 사용자가 특정 자원에 접근할 수 있는 권한이 있는지 확인하는 것.
방법 : 권한 부여, 권한 검사
예시 : 관리자만 접근할 수 있는 페이지에 접근하려고 할 때, 사용자가 관리자 권한이 있는지 확인하는 과정, 사용자의 권한을 변경하는 과정, 사용자가 특정 자원에 대한 권한이 있는지 확인하는 과정

*/
