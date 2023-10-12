using COM.ViVaKR.API;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

var builder = WebApplication.CreateBuilder(args);

// 추가 1
builder.Services.AddControllers();
builder.Services.AddDbContext<PaymentDetailContext>(options
=> options.UseSqlServer(builder.Configuration.GetConnectionString("DevConnection")));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    // 추가 2
    app.UseSwaggerUI(x =>
    {
        x.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        x.RoutePrefix = string.Empty;
    });
}

app.UseCors(options => options.WithOrigins("http://localhost:4200").AllowAnyMethod().AllowAnyHeader());

// 추가 5 : 자바스크립 용
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseHttpsRedirection();

// 추가 4
app.UseAuthentication();

// 추가 3
app.MapControllers();
app.Run();
