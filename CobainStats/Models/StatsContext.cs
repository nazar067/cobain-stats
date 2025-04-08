using Microsoft.EntityFrameworkCore;

namespace CobainStats.Models
{
    public class StatsContext: DbContext
    {
        public StatsContext(DbContextOptions<StatsContext> options) : base(options) { }
        public DbSet<DailyStat> DailyStats { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DailyStat>().HasNoKey().ToView(null);
        }
    }
}
