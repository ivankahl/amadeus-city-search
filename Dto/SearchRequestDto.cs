using FluentValidation;

namespace AmadeusCitySearch.Dto;

public record SearchRequestDto(string Query);

public class SearchRequestDtoValidator : AbstractValidator<SearchRequestDto>
{
    public SearchRequestDtoValidator()
    {
        RuleFor(x => x.Query)
            .NotEmpty().WithMessage("Please provide a valid search query")
            .MinimumLength(2).WithMessage("Please provide at least 2 characters in your search query")
            .MaximumLength(100).WithMessage("Please ensure your search query does not exceed 100 characters");
    }
}