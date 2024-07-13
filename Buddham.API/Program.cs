using Buddham.API.Data; // Add this line (3)
using Microsoft.AspNetCore.Identity; // Add this line (4)
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;

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

var sqlBuilder = new SqlConnectionStringBuilder
{
    DataSource = builder.Configuration["DataSource"],
    InitialCatalog = builder.Configuration["Database"],
    UserID = builder.Configuration["Database"],
    Password = builder.Configuration["DbPassword"],
    TrustServerCertificate = true
};
builder.Services.AddControllers(); // For API Controllers
builder.Services.AddEndpointsApiExplorer(); // For Swagger
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Scheme = "Bearer"
    });

    // Add this line (6), dotnet add package Swashbuckle.AspNetCore.Filters
    options.OperationFilter<SecurityRequirementsOperationFilter>();
});

// Add this line (7)
builder.Services.AddDbContext<DataContext>(options => options.UseSqlServer(sqlBuilder.ConnectionString));
// builder.Services.AddDbContext<DataContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DbConnection"))); // For Production
builder.Services.AddAuthorization(); // Add this line (1), For Identity
builder.Services.AddIdentityApiEndpoints<IdentityUser>().AddEntityFrameworkStores<DataContext>(); // Add this line (2), For Identity
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors(corsapp);

app.MapIdentityApi<IdentityUser>(); // Add this line (5), For Identity
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

// dotnet add package Swashbuckle.AspNetCore.Filters --version 8.0.2
