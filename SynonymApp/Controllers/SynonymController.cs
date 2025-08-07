using MediatR;
using SynonymApp.Application.Synonyms.Commands;
using SynonymApp.Application.Synonyms.Queries;

namespace SynonymApp.Controllers
{
    public class SynonymController : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            var message = app.NewVersionedApi();

            message.MapGet("api/v{version:apiVersion}/synonyms/{word}", async (string word, ISender sender) =>
            {
                return Results.Ok(await sender.Send(new GetSynonymsQuery
                {
                    Word = word
                }));
            })
                .RequireAuthorization()
                .HasApiVersion(1)
                .Produces(StatusCodes.Status200OK, typeof(IEnumerable<string>))
                .WithDescription("Get synonyms for a given word.");

            message.MapPost("api/v{version:apiVersion}/synonyms", async (AddSynonymCommand command, ISender sender) =>
            {
                await sender.Send(command);

                return Results.Created();
            })
                .RequireAuthorization()
                .HasApiVersion(1)
                .Produces(StatusCodes.Status201Created)
                .WithDescription("Add a synonym for a given word.");
        }
    }
}
