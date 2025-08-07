using FluentValidation;

namespace SynonymApp.Application.Auth.Commands
{
    public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
    {
        public RegisterCommandValidator()
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
