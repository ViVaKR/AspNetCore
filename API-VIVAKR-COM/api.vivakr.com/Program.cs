using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ViVaKR.API.Data;
using ViVaKR.API.Helpers;
using ViVaKR.API.Interfaces;
using ViVaKR.API.Models;
using ViVaKR.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Cors
const string corsapp = "corsapp";

builder.WebHost.UseUrls("http://localhost:55580"); // --> 배포환경, https://api.vivakr.com
// builder.WebHost.UseUrls("http://localhost:55570"); // --> 배포환경, https://api.vivakr.com

builder.Services.AddCors(options => options.AddPolicy(corsapp,
    policy => policy.WithOrigins(Helpers.AllowOrigins()).AllowAnyHeader().AllowAnyMethod()));

builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 50 * 1024 * 1024; // 50MB
});

builder.Services.AddDbContextPool<VivaKRDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
    {
        options.Password.RequiredLength = 8;
        options.User.RequireUniqueEmail = true;
        options.Lockout.AllowedForNewUsers = true;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
        options.Lockout.MaxFailedAccessAttempts = 3;
    }
).AddEntityFrameworkStores<VivaKRDbContext>().AddDefaultTokenProviders();

builder.Services.Configure<FormOptions>(options =>
{
    options.ValueLengthLimit = 10 * 1024 * 1024; // 10MB
    options.MultipartBodyLengthLimit = 50 * 1024 * 1024; // 50MB
    options.MultipartHeadersLengthLimit = 1024 * 1024; // 1MB
});

// 목적 : 파일 업로드 시 파일 크기 제한을 늘리기 위함
builder.Services.Configure<FormOptions>(options =>
{
    options.ValueLengthLimit = int.MaxValue;
    options.MultipartBodyLengthLimit = long.MaxValue;
    options.MultipartHeadersLengthLimit = int.MaxValue;
});

// 추가된 처리
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Events.OnRedirectToLogin = context =>
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        return Task.CompletedTask;
    };
});

//* (DI) JWT Configuration Injection
builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection("JWTConfig"));

try
{
    builder.Services.AddAuthentication(x =>
    {
        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; // 인증 성공 시 동작
        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; // 잘못된 인증 시 동작
        x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme; // 기본 스키마
    }).AddJwtBearer(x =>
    {
        var key = Encoding.ASCII.GetBytes(builder.Configuration["JWTConfig:Key"]!);
        var audience = builder.Configuration["JWTConfig:Audience"];
        var issuer = builder.Configuration["JWTConfig:Issuer"];

        x.SaveToken = true; // 토큰 저장 여부
        x.RequireHttpsMetadata = true; // https 사용 여부
        x.TokenValidationParameters = new TokenValidationParameters // 토큰 유효성 검사
        {
            ValidateIssuer = true, // 발급자 확인
            ValidateAudience = true, // 수취인 확인
            ValidateLifetime = true, // 토큰 만료 확인
            ClockSkew = TimeSpan.Zero, // 시간차
            ValidateIssuerSigningKey = true, // 발급자 서명 확인
            ValidAudience = audience, // 수취인
            ValidIssuer = issuer, // 발급자
            IssuerSigningKey = new SymmetricSecurityKey(key) // 발급자 서명 키
        };
    });
}
catch (InvalidOperationException ex)
{
    throw new Exception("Invalid Operation Exception: " + ex.Message, ex);
}
catch (Exception ex)
{
    throw new Exception("Error: " + ex.Message, ex);
}


builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

// Swagger
builder.Services.AddSwaggerGen(x =>
{
    x.SwaggerDoc("v1", new OpenApiInfo { Title = "ViVaKR API", Version = "v1" });

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

    var securityRequirement = new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            []
        }
    };

    x.AddSecurityRequirement(securityRequirement);
});

// Email Service
builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddHttpClient();

var app = builder.Build();

app.UseCors(corsapp);
if (app.Environment.IsDevelopment()) // 개발 환경에서만 Swagger 사용
{
    app.UseSwagger();
    app.UseSwaggerUI(x => { x.SwaggerEndpoint("/swagger/v1/swagger.json", "ViVaKR API v1"); });
}

if (!app.Environment.IsDevelopment()) app.UseHttpsRedirection(); // https로 리다이렉션 하겠다.


app.UseStaticFiles(); // 정적 파일 사용
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Resources")),
    RequestPath = "/Resources"
});

var codeImages = Path.Combine("Temp", "FileData", "Images", "Code");
var images = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), codeImages);

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(images),
    RequestPath = "/images"
});

var codeFiles = Path.Combine("Temp", "FileData", "Files", "Code");
var files = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), codeFiles);

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(files),
    RequestPath = "/files"
});

app.UseAuthentication(); // 인증
app.UseAuthorization(); // 권한
app.MapControllers();
app.Run();