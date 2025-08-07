using FluentValidation;
using MediatR;
using SynonymApp.Domain;
using SynonymApp.Domain.DbModels;

namespace SynonymApp.Application.Auth.Commands
{
    public class RegisterCommandHandler(SynonymsDbContext context) : IRequestHandler<RegisterCommand>
    {
        public async Task Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            if (context.Users.Any(u => u.Username == request.Username))
                throw new ValidationException("User already exists!");

            var user = new Users
            {
                Username = request.Username,
                Password = request.Password 
            };

            context.Users.Add(user);
            await context.SaveChangesAsync();
        }
    }
}
