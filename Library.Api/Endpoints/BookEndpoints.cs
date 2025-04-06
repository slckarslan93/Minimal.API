using FluentValidation.Results;
using Library.Api.Models;
using Library.Api.Services;
using Library.Api.Validator;
using Microsoft.AspNetCore.Authorization;

namespace Library.Api.Endpoints;

public static class BookEndpoints
{
    public static void UseBookEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("login", (JwtProvider jwtProvider) =>
        {
            return Results.Ok(new { Token = jwtProvider.CreateToken() });
        }).WithTags("Auth");

        app.MapPost("books", async (Book book, IBookService bookService, CancellationToken cancellationToken) =>
        {
            BookValidator validator = new();
            ValidationResult validationResult = validator.Validate(book);

            if (!validationResult.IsValid)
            {
                return Results.BadRequest(validationResult.Errors.Select(s => s.ErrorMessage));
            }

            var result = await bookService.CreateAsync(book, cancellationToken);
            if (!result) return Results.BadRequest("Someting went wrong");

            return Results.Created("GetBook", new { isbn = book.Isbn });
            //return Results.Created($"/books/{book.Isbn}",book);
        }).WithTags("Books");

        app.MapGet("books", [Authorize] async (IBookService bookService, CancellationToken cancellationToken) =>
        {
            var books = await bookService.GetAllAsync(cancellationToken);
            return Results.Ok(books);
        }).WithTags("Books");

        app.MapGet("books/{isbn}", async (string isbn, IBookService bookService, CancellationToken cancellationToken) =>
        {
            Book? book = await bookService.GetByIsbnAsync(isbn, cancellationToken);
            return Results.Ok(book);
        }).WithName("GetBook").WithTags("Books");

        app.MapGet("getBooksByTitle/{title}", async (string title, IBookService bookService, CancellationToken cancellationToken) =>
        {
            var books = await bookService.SearchByTitleAsync(title, cancellationToken);
            return Results.Ok(books);
        }).WithTags("Books");

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
        }).WithTags("Books");

        app.MapDelete("books/{isbn}", async (string isbn, IBookService bookService, CancellationToken cancellationToken) =>
        {
            var result = await bookService.DeleteAsync(isbn, cancellationToken);

            if (!result) return Results.BadRequest("Someting went wrong");

            return Results.Ok(new { Message = "Book delete is successfull" });
        }).WithTags("Books");
    }
}