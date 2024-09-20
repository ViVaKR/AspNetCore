
using KR.CO.TEXT.API;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//! (1) Cors
const string corsapp = "corsapp";

builder.Services.AddCors(x=> x.AddPolicy(corsapp, x=> {
    x.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
}));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.WebHost.UseUrls("https://localhost:23232");

builder.Services.AddDbContext<TodoContext>(x=> x.UseInMemoryDatabase("TodoList"));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

/*
    ! $ dotnet add package Microsoft.VisualStudio.Web.CodeGeneration.Design
    ! $ dotnet add package Microsoft.EntityFrameworkCore.Design
    ! $ dotnet add package Microsoft.EntityFrameworkCore.SqlServer
    ! $ dotnet add package Microsoft.EntityFrameworkCore.Tools
    ! $ dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
    ! $ dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore
    ! $ dotnet add package Microsoft.AspNetCore.Mvc.NewtonsoftJson
    ! $ dotnet add package BCrypt.Net-Next

    ! $ dotnet tool uninstall -g dotnet-aspnet-codegenerator
    ! $ dotnet tool install -g dotnet-aspnet-codegenerator
    ! $ dotnet tool update -g dotnet-aspnet-codegenerator

    # Routing
    ==> Action
        1. GetAllStudents
        2. GetStudentById
        3. DeleteStudentById


    ==> Verb
        1. HttpGet
        2. HttpGet
        3. HttpDelete


    ==> Route
        1. https://localhost/api/stutdent
        2. https://localhost/api/student/1
        3. https://localhost/api/student/1
*/
