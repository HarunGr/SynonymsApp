using FluentValidation;

namespace SynonymApp.Application.Auth.Queries
{
    public class LoginQueryValidator : AbstractValidator<LoginQuery>
    {
        public LoginQueryValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty()
                .WithMessage("Username cannot be empty!");

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Password cannot be empty!");
        }
    }
}
