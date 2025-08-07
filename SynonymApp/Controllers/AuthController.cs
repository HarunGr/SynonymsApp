using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using SynonymApp.Application.Auth.Commands;
using SynonymApp.Application.Auth.Queries;

namespace SynonymApp.Controllers
{
    public class AuthController : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            var message = app.NewVersionedApi();

            message.MapPost("api/v{version:apiVersion}/register", async (RegisterCommand request, ISender sender) =>
            {
                await sender.Send(request);
                return Results.Created();
            })
                .HasApiVersion(1)
                .Produces(StatusCodes.Status201Created)
                .WithDescription("Register a user!");

            message.MapPost("api/v{version:apiVersion}/login", async (LoginQuery request, ISender sender) =>
            {
                return await sender.Send(request);
            })
                .HasApiVersion(1)
                .Produces(StatusCodes.Status200OK, typeof(string))
                .WithDescription("Login");
        }
    }
}
