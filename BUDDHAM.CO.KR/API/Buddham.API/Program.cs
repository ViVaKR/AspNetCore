using System.Text;
using Buddham.API.Data; // Add this line (3)
using Buddham.API.Models;
using Buddham.API.Service;
using Buddham.SharedLib.Contracts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity; // Add this line (4)
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Cors
const string corsapp = "corsapp";
builder.Services.AddCors(options =>
{
    options.AddPolicy(corsapp, builder =>
    {
        builder.WithOrigins("*").AllowAnyHeader().AllowAnyMethod();
    });
});

builder.WebHost.UseUrls("https://localhost:48391"); // 배포시 사용.
// builder.WebHost.UseUrls("https://localhost:48591"); // 개발시 사용.
var JWTConfig = builder.Configuration.GetSection(nameof(Buddham.API.Models.JWTConfig));


builder.Services.AddDbContext<DataContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ----------------------- Starting ----------------------- //

// 1.입증, 인증.
// 2.〔컴퓨터〕 인증: 정보에 접근할 수 있는 자격의 유무를 검증하는 절차.

//--> (1) Identity
builder.Services.AddIdentity<AppUser, IdentityRole>().AddEntityFrameworkStores<DataContext>().AddDefaultTokenProviders();
try
{
    builder.Services.Configure<JWTConfig>(builder.Configuration.GetSection(nameof(JWTConfig)));
    //--> (2) JWT
    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true, // 발급자 확인
            ValidateAudience = true, // 수취인 확인
            ValidateLifetime = true, // 유효기간 확인
            ClockSkew = TimeSpan.Zero, // 시간차이 확인
            ValidateIssuerSigningKey = true, // 서명 확인
            ValidAudience = JWTConfig.GetSection("Audience").Value, // builder.Configuration["Jwt:Issuer"], // 수취인,
            ValidIssuer = JWTConfig.GetSection("Issuer").Value, // builder.Configuration["Jwt:Issuer"], // 발급자,

            IssuerSigningKey =
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JWTConfig.GetSection("Key").Value ?? throw new InvalidOperationException("Secret Key is not found")) // 서명키
            )
        };
    });

}
catch (InvalidOperationException ex)
{
    throw new Exception(ex.Message);
}
catch (Exception ex)
{
    throw new Exception($"Secret Key is not found {ex.Message}");
}

builder.Services.AddControllers(); // For API Controllers
builder.Services.AddEndpointsApiExplorer(); // For Swagger

//--> Swagger Configuration
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization", // 이름
        In = ParameterLocation.Header, // 헤더에 토큰을 넣어주겠다.
        Type = SecuritySchemeType.ApiKey, // 타입

        // 생략 가능
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"", // 설명
        Scheme = "Bearer" // 스키마
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});

// ----------------------- Ending ----------------------- //
// builder.Services.AddScoped<IEmailSender, EmailSender>();
builder.Services.AddTransient<IEmailService, EmailService>();
var app = builder.Build();

if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection(); // https로 리다이렉션 하겠다.
}

var options = new DefaultFilesOptions(); // 기본 파일 옵션
options.DefaultFileNames.Clear(); // 기본 파일 이름 초기화
options.DefaultFileNames.Add("index.html"); // index.html 파일을 기본 파일로 설정
app.UseDefaultFiles(options); // 기본 파일 사용하겠다.
app.UseStaticFiles(); // 정적 파일 사용하겠다.

app.UseCors(corsapp);

//--> Authentication : 인증, 로그인 등의 절차를 거쳐 사용자의 신원을 확인하는 것
//--> Authorization : 허가, 로그인 후 사용자가 특정 자원에 접근할 수 있는 권한이 있는지 확인하는 것

app.UseAuthentication(); // 인증을 사용하겠다.
app.UseAuthorization(); // 권한 부여를 사용하겠다.
app.MapDefaultControllerRoute(); // 기본 컨트롤러 라우팅 사용하겠다.
app.MapControllers(); // 컨트롤러 사용하겠다.

app.Run();


/*
--> https://jwt.io : JWT 토큰 생성 및 디코딩 온라인 사이트
 */
