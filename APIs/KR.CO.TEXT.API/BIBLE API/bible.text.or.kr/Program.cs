using System.Text;
using Bible.API.Data;
using Bible.API.Interfaces;
using Bible.API.Models;
using Bible.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
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
        builder.WithOrigins("*")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.WebHost.UseUrls("https://localhost:55541"); // 배포시 사용
// builder.WebHost.UseUrls("https://localhost:55531"); // 개발시 사용

var JWTConfig = builder.Configuration.GetSection("JWTConfig").Get<JWTConfig>();


//* (DI) DbContext Configuration Injection
builder.Services.AddDbContext<BibleContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

//* (DI) Identity Configuration Injection
builder.Services.AddIdentity<AppUser, IdentityRole>()
.AddEntityFrameworkStores<BibleContext>()
.AddDefaultTokenProviders();

//* (DI) JWT Configuration Injection
builder.Services.Configure<JWTConfig>(builder.Configuration.GetSection("JWTConfig"));

//* JWT
try
{
    builder.Services.Configure<JWTConfig>(builder.Configuration.GetSection("JWTConfig"));
    builder.Services.AddAuthentication(x =>
    {
        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(x =>
    {
        var key = Encoding.ASCII.GetBytes(builder.Configuration["JWTConfig:Key"]!);
        var issuer = builder.Configuration["JWTConfig:Issuer"];
        var audience = builder.Configuration["JWTConfig:Audience"];

        x.SaveToken = true;
        x.RequireHttpsMetadata = false;
        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            ValidateIssuerSigningKey = true,
            ValidAudience = builder.Configuration["JWTConfig:Audience"],
            ValidIssuer = builder.Configuration["JWTConfig:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(key),
        };
    });
}
catch (InvalidOperationException ex)
{
    throw new Exception(ex.Message);
}
catch (Exception ex)
{
    throw new Exception(ex.Message);
}

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Add Swagger
// Swagger
builder.Services.AddSwaggerGen(x =>
{
    x.SwaggerDoc("v1", new OpenApiInfo() { Title = "Bible API", Version = "v1" });

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

builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddHttpClient();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(x =>
    {
        x.SwaggerEndpoint("/swagger/v1/swagger.json", "Bible API V1");
        // x.RoutePrefix = string.Empty;
    });
}
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection(); // https로 리다이렉션 하겠다.
}

// https://localhost:55541/swagger/index.html
app.UseCors(corsapp);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
