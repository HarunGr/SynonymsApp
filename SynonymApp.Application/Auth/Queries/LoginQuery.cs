using MediatR;

namespace SynonymApp.Application.Auth.Queries
{
    public class LoginQuery : IRequest<string>
    {
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
