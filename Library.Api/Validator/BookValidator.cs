using FluentValidation;
using Library.Api.Models;

namespace Library.Api.Validator;

public class BookValidator : AbstractValidator<Book>
{
    public BookValidator()
    {
        RuleFor(x => x.Isbn)
            .NotEmpty()
            .WithMessage("Isbn is required")
            .Length(10, 13)
            .WithMessage("Isbn must be between 10 and 13 characters");

        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title is required")
            .Length(1, 100)
            .WithMessage("Title must be between 1 and 100 characters");

        RuleFor(x => x.ShortDescription)
            .NotEmpty()
            .WithMessage("ShortDescription is required")
            .Length(1, 500)
            .WithMessage("ShortDescription must be between 1 and 500 characters");

        RuleFor(x => x.PageCount)
            .GreaterThan(0)
            .WithMessage("PageCount must be greater than 0");

        RuleFor(x => x.PublishDate)
            .LessThan(DateTime.Now)
            .WithMessage("PublishDate must be in the past");
    }
}

