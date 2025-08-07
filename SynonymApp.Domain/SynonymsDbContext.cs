using Microsoft.EntityFrameworkCore;
using SynonymApp.Domain.DbModels;

namespace SynonymApp.Domain
{
    public class SynonymsDbContext : DbContext
    {
        public SynonymsDbContext(DbContextOptions<SynonymsDbContext> options) : base(options)
        {

        }

        public DbSet<Synonyms> Synonyms { get; set; }
        public DbSet<Users> Users { get; set; }
    }
}
