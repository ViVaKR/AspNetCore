using ContosoPizza;
using ContosoPizza.Data;
using ContosoPizza.Services;
// Additional using declarations

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add the PizzaContext
// PizzaContext 를 ASP.NET Core 종속성 삽입 시스템에 등록
// PizzaContext 가 SQLite 데이터베이스 공급가를 사용하도록 함.
// 로컬파일 ContosePizz.db 를 가리키는 SQLite 연결 문자열을 정의.
// 로컬 개발의 경우 비밀번호 관리자를 사용.
// 프로덕션 배포의 경우 Azure Key Vault 서비스를 사용.
builder.Services.AddSqlite<PizzaContext>("Data Source=ContosPizza.db");

// Add the PromotionsContext
builder.Services.AddSqlite<PromotionsContext>("Data Source=Promotions/Promotions.db");
// Promotions/Promotions.db
builder.Services.AddScoped<PizzaService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

// Add the CreateDbIfNotExists method call
// app.CreateDbIfNotExists();

app.MapGet("/", () => @"Contoso Pizza management API. Navigate to /swagger to open the Swagger test UI.");

app.Run();


// 스캐폴딩

/*
$ dotnet ef dbcontext scaffold "Data Source=Promotions/Promotions.db" Microsoft.EntityFrameworkCore.Sqlite --context-dir Data --output-dir Models
 */
