using MediatR;

namespace SynonymApp.Application.Synonyms.Queries
{
    public class GetSynonymsQuery : IRequest<IEnumerable<string>>
    {
        public string Word { get; set; } = null!;
    }
}
