using MediatR;
using Microsoft.Extensions.Caching.Memory;
using SynonymApp.Domain.Constants;
using System.Reflection.Metadata;

namespace SynonymApp.Application.Synonyms.Queries
{
    public class GetSynonymsQueryHandler(IMemoryCache cache) : IRequestHandler<GetSynonymsQuery, IEnumerable<string>>
    {

        public async Task<IEnumerable<string>> Handle(GetSynonymsQuery request, CancellationToken cancellationToken)
        {
            var graph = cache.Get<Dictionary<string, HashSet<string>>>(CacheKeyConstants.CacheKey);

            var resultSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            if (graph is null || !graph.ContainsKey(request.Word)) 
            {
                return resultSet;
            }

            var stack = new Stack<string>();

            stack.Push(request.Word);

            while (stack.Count > 0)
            {
                var currentWord = stack.Pop();
                if (!resultSet.Add(currentWord)) continue;

                if (graph.TryGetValue(currentWord, out var neighbourWords))
                {
                    foreach (var neighbourWord in neighbourWords)
                    {
                        stack.Push(neighbourWord);
                    }
                }
            }

            resultSet.Remove(request.Word);

            return resultSet;
        }
    }
}
