using MediatR;

namespace SynonymApp.Application.Auth.Commands
{
    public class RegisterCommand : IRequest
    {
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
