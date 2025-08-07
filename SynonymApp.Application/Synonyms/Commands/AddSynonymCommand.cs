using MediatR;

namespace SynonymApp.Application.Synonyms.Commands
{
    public class AddSynonymCommand : IRequest
    {
        public string Word { get; set; } = null!;
        public string Synonym { get; set; } = null!;
    }
}
