using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ViVaBM.API.Data;
using ViVaBM.API.Helpers;
using ViVaBM.API.Interfaces;
using ViVaBM.API.Models;
using ViVaBM.API.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var defaultConnection = builder.Configuration["DefaultConnection"];

// Cors
const string corsapp = "corsapp";
builder.Services.AddCors(options => options.AddPolicy(corsapp, policy => policy.WithOrigins("*").AllowAnyHeader().AllowAnyMethod()));

try
{
    builder.Services.AddAuthentication(x =>
    {
        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; // 인증 성공 시 동작
        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; // 잘못된 인증 시 동작
        x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme; // 기본 스키마
    }).AddJwtBearer(x =>
    {
        x.SaveToken = true; // 토큰 저장 여부
        x.RequireHttpsMetadata = false; // https 사용 여부
        x.TokenValidationParameters = new TokenValidationParameters // 토큰 유효성 검사
        {
            ValidateIssuer = true, // 발급자 확인
            ValidateAudience = true, // 수취인 확인
            ValidateLifetime = true, // 토큰 만료 확인
            ClockSkew = TimeSpan.Zero, // 시간차
            ValidateIssuerSigningKey = true, // 발급자 서명 확인
            ValidAudience = AuthSettings.Audience, // 수취인
            ValidIssuer = AuthSettings.Issuer, // 발급자

            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AuthSettings.PrivateKey)) // 발급자 서명 키
        };

    });
}
catch (InvalidOperationException ex)
{
    throw new Exception("Invalid Operation Exception: ", ex);
}
catch (Exception ex)
{
    throw new Exception("Error: ", ex);
}

// builder.Services.AddDbContextPool<VivabmDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))); // UseNpgsql
builder.Services.AddDbContextPool<VivabmDbContext>(options => options.UseNpgsql(defaultConnection));

// Identity
builder.Services.AddIdentity<AppUser, IdentityRole>()
.AddEntityFrameworkStores<VivabmDbContext>().AddDefaultTokenProviders();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(x =>
{
    x.SwaggerDoc("v1", new OpenApiInfo() { Title = "Writer API", Version = "v1" });

    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "JWT Authentication",
        Description = "Enter your JWT token in this fiedl",
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

// Email Service
builder.Services.AddTransient<IEmailService, EmailService>();

var app = builder.Build();

app.UseAuthentication(); // 인증

app.UseAuthorization(); // 권한

if (app.Environment.IsDevelopment()) // 개발 환경에서만 Swagger 사용
{
    app.UseSwagger();
    app.UseSwaggerUI(x =>
    {
        x.SwaggerEndpoint("/swagger/v1/swagger.json", "ViVaBM API v1");
        // x.RoutePrefix = string.Empty;
    });
}

app.Urls.Add("https://localhost:50021");
app.UseCors(corsapp);
app.MapControllers();

app.Run();