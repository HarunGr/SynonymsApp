
using SynonymApp.Domain;
using SynonymApp.Domain.DbModels;
using SynonymApp.Infrastructure.BackgroundJobs;
using System.Text.Json;

namespace SynonymApp.Background
{
    public class SynonymsPersistenceBackgroundService : BackgroundService
    {
        private readonly SynonymsPersistenceChannel _channel;
        private readonly IServiceProvider _serviceProvider;
        public SynonymsPersistenceBackgroundService(SynonymsPersistenceChannel channel, IServiceProvider serviceProvider)
        {
            _channel = channel;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await foreach (var stringSet in _channel.Reader.ReadAllAsync(stoppingToken))
            {
                try
                {
                    var dbContext = _serviceProvider.GetRequiredService<SynonymsDbContext>();

                    var json = JsonSerializer.Serialize(stringSet); // Dictionary<string, HashSet<string>>
                    var entity = new Synonyms { GraphJson = json };

                    // Optional: Clear old graph (if only one is stored)
                    dbContext.Synonyms.RemoveRange(dbContext.Synonyms);
                    dbContext.Synonyms.Add(entity);
                    await dbContext.SaveChangesAsync();
                }
                catch (Exception e)
                {
                }
            }
        }
    }
}
