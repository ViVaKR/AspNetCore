using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using TodoApi.Data;
using TodoApi.DTOs;
using TodoApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Cors
const string corsapp = "corsapp";
builder.Services.AddCors(options => options.AddPolicy(corsapp, builder => builder.WithOrigins("*").AllowAnyHeader().AllowAnyMethod()));

builder.WebHost.UseUrls("https://localhost:51034");
builder.Services.AddDbContext<TodoDb>(opt => opt.UseInMemoryDatabase("TodoList"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    // --> UI, https://localhost:51034/scalar/v1
    // --> Json, https://localhost:51034/openapi/v1
    app.MapScalarApiReference();
}

app.UseCors(corsapp);

var todoItems = app.MapGroup("/todoitems");

todoItems.MapGet("/", GetAllTodos);
todoItems.MapGet("/completed", GetCompleteTodos);
todoItems.MapGet("/{id}", GetTodo);
todoItems.MapPost("/", CreateTodo);
todoItems.MapPut("/{id}", UpdateTodo);
todoItems.MapDelete("/{id}", DeleteTodo);

app.MapGet("/", () => "Hello World!");

app.UseHttpsRedirection();

await app.RunAsync();

static async Task<IResult> GetAllTodos(TodoDb db)
{
    return TypedResults.Ok(await
    db.Todos.Select(x => new TodoItemDTO(x)).ToArrayAsync());
}

static async Task<IResult> GetCompleteTodos(TodoDb db)
{
    return TypedResults.Ok(await
    db.Todos.Where(x => x.IsComplete).Select(x => new TodoItemDTO(x)).ToListAsync());
}

static async Task<IResult> GetTodo(int id, TodoDb db)
{
    return await db.Todos.FindAsync(id) is Todo todo
    ? TypedResults.Ok(new TodoItemDTO(todo))
    : TypedResults.NotFound();
}

static async Task<IResult> CreateTodo(TodoItemDTO todoItemDTO, TodoDb db)
{
    var todoItem = new Todo
    {
        Name = todoItemDTO.Name,
        IsComplete = todoItemDTO.IsComplete
    };

    db.Todos.Add(todoItem);

    await db.SaveChangesAsync();

    todoItemDTO = new TodoItemDTO(todoItem);

    return TypedResults.Created($"/todoitems/{todoItem.Id}", todoItemDTO);
}

static async Task<IResult> UpdateTodo(int id, TodoItemDTO todoItemDTO, TodoDb db)
{
    var todo = await db.Todos.FindAsync(id);

    if (todo is null) return TypedResults.NotFound();

    todo.Name = todoItemDTO.Name;
    todo.IsComplete = todoItemDTO.IsComplete;

    await db.SaveChangesAsync();
    return TypedResults.NoContent();
}

static async Task<IResult> DeleteTodo(int id, TodoDb db)
{
    if (await db.Todos.FindAsync(id) is Todo todo)
    {
        db.Todos.Remove(todo);
        await db.SaveChangesAsync();
        return TypedResults.NoContent();
    }

    return TypedResults.NotFound();
}







/*
# OpenSSL generates a secret of 129 bytes.
# 129 bytes is good for HS512 (see https://github.com/ueberauth/guardian/issues/152).
--> openssl rand -base64 129 | tr -d '\n'
--> openssl rand -base64 129 | tr -d '\n' | pbcopy

HS256 - HMAC using SHA-256
HS384 - HMAC using SHA-384
HS512 - HMAC using SHA-512
 */


