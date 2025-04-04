using System.Threading;
using FluentValidation.Results;
using Library.Api.Context;
using Library.Api.Models;
using Library.Api.Services;
using Library.Api.Validator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true);

builder.Services.AddAuthentication().AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "Issuer",
            ValidAudience ="Audience",
            IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes("my secret key mysecret key my secret key mysecret key my secret key mysecret key my secret key mysecret key"))
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<JwtProvider>();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseInMemoryDatabase("MyDb");
});


var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.MapGet("login", (JwtProvider jwtProvider) =>
{
    return Results.Ok(new { Token = jwtProvider.CreateToken() }); 
});

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

    return Results.Created("GetBook", new { isbn = book.Isbn});
    //return Results.Created($"/books/{book.Isbn}",book);
});


app.MapGet("books",[Authorize] async (IBookService bookService , CancellationToken cancellationToken) =>
{
    var books = await bookService.GetAllAsync(cancellationToken);
    return Results.Ok(books);
});

app.MapGet("books/{isbn}", async (string isbn, IBookService bookService, CancellationToken cancellationToken) =>
{
    Book? book = await bookService.GetByIsbnAsync(isbn, cancellationToken);
    return Results.Ok(book);
}).WithName("GetBook");

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
