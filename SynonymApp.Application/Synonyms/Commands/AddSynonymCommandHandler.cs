using MediatR;
using Microsoft.Extensions.Caching.Memory;
using SynonymApp.Domain.Constants;
using SynonymApp.Infrastructure.BackgroundJobs;

namespace SynonymApp.Application.Synonyms.Commands
{
    public class AddSynonymCommandHandler(IMemoryCache cache, SynonymsPersistenceChannel channel) : IRequestHandler<AddSynonymCommand>
    {
        public async Task Handle(AddSynonymCommand request, CancellationToken cancellationToken)
        {
            if (!cache.TryGetValue(CacheKeyConstants.CacheKey, out Dictionary<string, HashSet<string>> graph))
            {
                graph = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);
            }

            AddLink(graph, request.Word, request.Synonym);
            AddLink(graph, request.Synonym, request.Word);

            cache.Set(CacheKeyConstants.CacheKey, graph);

            await channel.Writer.WriteAsync(graph);
        }

        private void AddLink(Dictionary<string, HashSet<string>> graph, string word, string synonym) 
        {
            if(!graph.TryGetValue(word, out var set))
            {
                set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                graph[word] = set;
            }

            set.Add(synonym);
        }
    }
}
