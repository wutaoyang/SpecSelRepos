using Microsoft.EntityFrameworkCore;

namespace SpecSelRepos.Models
{
    public class SpecSelResultContext : DbContext
    {
        public SpecSelResultContext(DbContextOptions<SpecSelResultContext> options)
                : base(options)
        {
        }

        public DbSet<SpecSelResult> SpecSelResult { get; set; }
    }
}
