using System.Text;
using Buddham.API.Data;
using Buddham.API.Models;
using Buddham.API.Service;
using Buddham.SharedLib.Contracts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Cors
const string corsapp = "corsapp";
builder.Services.AddCors(options => options.AddPolicy(corsapp, builder => builder.WithOrigins("*").AllowAnyHeader().AllowAnyMethod()));

builder.WebHost.UseUrls("https://localhost:48491"); // 배포시.
// builder.WebHost.UseUrls("https://localhost:48791"); // 개발시.

var jwtConfig = builder.Configuration.GetSection(nameof(JWTConfig)).Get<JWTConfig>();

//* (DI) DbContext Configuration Injection
builder.Services.AddDbContext<BuddhaContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

//* (DI) Identity Configuration Injection
builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    options.Lockout.AllowedForNewUsers = true; // 신규 사용자에 대한 잠금 허용
    options.Lockout.MaxFailedAccessAttempts = 5; // 최대 실패 횟수
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // 기본 잠금 시간
    // options.Password.RequireDigit = true; // 숫자 필요
    // options.Password.RequiredLength = 8; // 최소 길이
    // options.Password.RequireLowercase = true; // 소문자 필요
    // options.Password.RequireNonAlphanumeric = true; // 특수문자 필요
    // options.Password.RequireUppercase = true; // 대문자 필요
    // options.User.RequireUniqueEmail = true; // 이메일 중복 방지
    // options.SignIn.RequireConfirmedEmail = true; // 이메일 확인 필요
    // options.SignIn.RequireConfirmedAccount = true; // 계정 확인 필요
    // options.Tokens.EmailConfirmationTokenProvider = "emailconfirmation"; // 이메일 확인 토큰 제공자
    // options.Tokens.PasswordResetTokenProvider = "passwordreset"; // 비밀번호 재설정 토큰 제공자
}).AddEntityFrameworkStores<BuddhaContext>().AddDefaultTokenProviders();

builder.Services.Configure<FormOptions>(options =>
{
    options.ValueLengthLimit = int.MaxValue;
    options.MultipartBodyLengthLimit = int.MaxValue;
    options.MemoryBufferThreshold = int.MaxValue;
});

//--> (2) 인증 서비스 추가
try
{
    //* (DI) JWT Configuration Injection
    builder.Services.Configure<JWTConfig>(builder.Configuration.GetSection(nameof(JWTConfig)));

    //--> (2) JWT
    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(options =>
    {
        var key = Encoding.ASCII.GetBytes(jwtConfig?.Key!);
        var audience = jwtConfig?.Audience!;
        var issuer = jwtConfig?.Issuer!;

        options.SaveToken = true;
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true, // 발급자 확인
            ValidateAudience = true, // 수취인 확인
            ValidateLifetime = true, // 유효기간 확인
            ClockSkew = TimeSpan.Zero, // 시간차이 확인
            ValidateIssuerSigningKey = true, // 서명 확인
            ValidAudience = audience, // 수취인
            ValidIssuer = issuer, // 발급자
            IssuerSigningKey = new SymmetricSecurityKey(key) // 서명키
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
builder.Services.AddSwaggerGen(x =>
{
    x.SwaggerDoc("v1", new OpenApiInfo() { Title = "Buddha API", Version = "v1" });
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "JWT Authentication",
        Description = "Enter your JWT token in this field",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    };

    x.AddSecurityDefinition("Bearer", securityScheme);

    var securityRequirement = new OpenApiSecurityRequirement{
        {
            new OpenApiSecurityScheme{
                Reference = new OpenApiReference{
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    };

    x.AddSecurityRequirement(securityRequirement);

});

// ----------------------- Ending ----------------------- //
// builder.Services.AddScoped<IEmailSender, EmailSender>();
builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddHttpClient();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(x =>
    {
        x.SwaggerEndpoint("/swagger/v1/swagger.json", "Buddham API V1");
        // x.RoutePrefix = string.Empty;
    });
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection(); // https로 리다이렉션 하겠다.
}

// var options = new DefaultFilesOptions(); // 기본 파일 옵션
// options.DefaultFileNames.Clear(); // 기본 파일 이름 초기화
// options.DefaultFileNames.Add("index.html"); // index.html 파일을 기본 파일로 설정
// app.UseDefaultFiles(options); // 기본 파일 사용하겠다.

app.UseCors(corsapp);

app.UseStaticFiles(); // 정적 파일 사용하겠다.
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"Resources")),
    RequestPath = "/Resources"

});

app.UseAuthentication(); // 인증을 사용하겠다.
app.UseAuthorization(); // 권한 부여를 사용하겠다.
app.MapControllers(); // 컨트롤러 사용하겠다.

app.Run();

/*
string mongoEndpoint = Environment.GetEnvironmentVariable("MONGO_ENDPOINT");

--> https://jwt.io : JWT 토큰 생성 및 디코딩 온라인 사이트
 */
