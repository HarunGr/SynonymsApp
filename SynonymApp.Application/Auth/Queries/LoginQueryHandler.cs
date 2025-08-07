using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using SynonymApp.Domain;
using SynonymApp.Domain.DbModels;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SynonymApp.Application.Auth.Queries
{
    public class LoginQueryHandler(SynonymsDbContext context) : IRequestHandler<LoginQuery, string>
    {
        public async Task<string> Handle(LoginQuery request, CancellationToken cancellationToken)
        {
            var user = await context.Users.FirstOrDefaultAsync(u =>
            u.Username == request.Username &&
            u.Password == request.Password); 

            if (user is null)
                throw new ValidationException("User does not exist!");

            return GenerateJwtToken(user);
        }

        private string GenerateJwtToken(Users user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("b5498854e238943a22c1f8ccfd4d8523b72b6e8880520e2eb7e72dbc06a17887"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
            new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub, user.Username),
            new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

            var token = new JwtSecurityToken(
                issuer: "SynonymAuth",
                audience: "SynonymApp",
                claims: claims,
                expires: DateTime.UtcNow.AddMonths(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
