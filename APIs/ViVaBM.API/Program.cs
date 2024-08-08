using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ViVaBM.API.Data;
using ViVaBM.API.Helpers;
using ViVaBM.API.Models;

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
        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

    }).AddJwtBearer(x =>
    {
        x.RequireHttpsMetadata = false;
        x.SaveToken = true;
        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            ValidateIssuerSigningKey = true,
            ValidAudience = AuthSettings.Audience, // 수취인
            ValidIssuer = AuthSettings.Issuer, // 발급자

            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AuthSettings.PrivateKey))
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

// builder.Services.AddDbContextPool<VivabmDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
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


var app = builder.Build();

app.UseAuthentication();

app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(x =>
    {
        x.SwaggerEndpoint("/swagger/v1/swagger.json", "ViVaBM API v1");
        // x.RoutePrefix = string.Empty;
    });
}

app.MapGet("/helloworld", () => "Hello World!");

app.Urls.Add("https://localhost:50021");
app.UseCors(corsapp);
app.MapControllers();

app.Run();
