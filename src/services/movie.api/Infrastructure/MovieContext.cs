using Microsoft.EntityFrameworkCore;
using movie.api.Infrastructure.EntityConfigurations;
using movie.data.objects;

namespace movie.api.Infrastructure
{
    public class MovieContext : DbContext
    {
        public MovieContext(DbContextOptions<MovieContext> options) : base(options)
        { }

        public DbSet<Movie> Movies { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new MovieEntityTypeConfiguration());
            builder.HasDefaultSchema("public");
            base.OnModelCreating(builder);
        }
    }
}
