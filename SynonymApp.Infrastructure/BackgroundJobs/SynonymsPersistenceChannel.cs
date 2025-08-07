using System.Threading.Channels;

namespace SynonymApp.Infrastructure.BackgroundJobs
{
    public class SynonymsPersistenceChannel
    {
        public Channel<Dictionary<string, HashSet<string>>> _channel { get; } = Channel.CreateUnbounded<Dictionary<string, HashSet<string>>>();
        public ChannelReader<Dictionary<string, HashSet<string>>> Reader => _channel.Reader;
        public ChannelWriter<Dictionary<string, HashSet<string>>> Writer => _channel.Writer;
    }
}
