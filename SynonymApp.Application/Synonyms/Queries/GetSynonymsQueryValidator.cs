using FluentValidation;

namespace SynonymApp.Application.Synonyms.Queries
{
    public class GetSynonymsQueryValidator : AbstractValidator<GetSynonymsQuery>
    {
        public GetSynonymsQueryValidator()
        {
            RuleFor(x => x.Word)
                .NotEmpty()
                .WithMessage("Search term must not be empty!");
        }
    }
}
