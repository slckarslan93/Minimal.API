using Library.Api.Context;
using Library.Api.Models;
using Library.Api.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IBookService, BookService>();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseInMemoryDatabase("MyDb");
});


var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapPost("books", async (Book book, IBookService bookService) =>
{
    var result = await bookService.CreateAsync(book);
    if (!result) return Results.BadRequest("Someting went wrong");

    return Results.Ok(result);
    
});


app.Run();
