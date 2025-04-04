using System.Threading;
using FluentValidation.Results;
using Library.Api.Context;
using Library.Api.Models;
using Library.Api.Services;
using Library.Api.Validator;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true);

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

app.MapPost("books", async (Book book, IBookService bookService ,CancellationToken cancellationToken) =>
{
    BookValidator validator = new();
    ValidationResult validationResult =  validator.Validate(book);

    if (!validationResult.IsValid)
    {
        return Results.BadRequest(validationResult.Errors.Select(s => s.ErrorMessage));
    }

    var result = await bookService.CreateAsync(book, cancellationToken);
    if (!result) return Results.BadRequest("Someting went wrong");

    return Results.Ok(new {Message = "Book create is successfull" });
});


app.MapGet("books", async (IBookService bookService , CancellationToken cancellationToken) =>
{
    var books = await bookService.GetAllAsync(cancellationToken);
    return Results.Ok(books);
});

app.MapGet("books/{isbn}", async (string isbn, IBookService bookService, CancellationToken cancellationToken) =>
{
    Book? book = await bookService.GetByIsbnAsync(isbn, cancellationToken);
    return Results.Ok(book);
});

app.MapGet("getBooksByTitle/{title}", async (string title, IBookService bookService, CancellationToken cancellationToken) =>
{
    var books = await bookService.SearchByTitleAsync(title, cancellationToken);
    return Results.Ok(books);
});

app.MapPut("books", async (Book book, IBookService bookService, CancellationToken cancellationToken) =>
{
    BookValidator validator = new();
    ValidationResult validationResult = validator.Validate(book);

    if (!validationResult.IsValid)
    {
        return Results.BadRequest(validationResult.Errors.Select(s => s.ErrorMessage));
    }

    var result = await bookService.UpdateAsync(book, cancellationToken);
    if (!result) return Results.BadRequest("Someting went wrong");

    return Results.Ok(new { Message = "Book update is successfull" });
});

app.MapDelete("books/{isbn}", async (string isbn, IBookService bookService, CancellationToken cancellationToken) =>
{
    var result = await bookService.DeleteAsync(isbn, cancellationToken);

    if(!result) return Results.BadRequest("Someting went wrong");

    return Results.Ok(new { Message = "Book delete is successfull" });
});



app.Run();
