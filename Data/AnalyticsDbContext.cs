using Microsoft.EntityFrameworkCore;
using PortfolioAnalyticsApi.Models;

namespace PortfolioAnalyticsApi.Data;

public class AnalyticsDbContext : DbContext
{
    public AnalyticsDbContext(DbContextOptions<AnalyticsDbContext> options) : base(options)
    {
    }

    public DbSet<AnalyticsEvent> AnalyticsEvents { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<AnalyticsEvent>(entity =>
        {
            entity.HasIndex(e => e.Timestamp);
            entity.HasIndex(e => e.SessionId);
            entity.HasIndex(e => e.EventType);
            entity.HasIndex(e => new { e.PageUrl, e.Timestamp });
        });
    }
}
