using FluentValidation.Results;
using Library.Api.Context;
using Library.Api.Models;
using Library.Api.Services;
using Library.Api.Validator;
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


app.Run();
