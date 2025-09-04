using Microsoft.EntityFrameworkCore;
using analytics_dashboard.models;

namespace analytics_dashboard.data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Article> Articles { get; set; }
        public DbSet<ArticleDetail> ArticleDetails { get; set; }
        public DbSet<Pageview> Pageviews { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Article>(entity =>
{
    entity.HasKey(a => a.Id);
    entity.Property(a => a.Title).HasMaxLength(200).IsRequired();
    entity.Property(a => a.Category).HasMaxLength(100).IsRequired();
    entity.Property(a => a.PublishedAt)
          .IsRequired()
          .HasColumnType("timestamp without time zone");
});

            modelBuilder.Entity<ArticleDetail>(entity =>
            {
                entity.HasKey(ad => ad.ArticleId);
                entity.Property(ad => ad.Summary).HasMaxLength(500);
                entity.Property(ad => ad.HeroImageUrl).HasMaxLength(300);
                
                entity.HasOne(ad => ad.Article)
                      .WithOne(a => a.ArticleDetail)
                      .HasForeignKey<ArticleDetail>(ad => ad.ArticleId);
            });

            modelBuilder.Entity<Pageview>(entity =>
{
    entity.HasKey(p => p.Id);
    entity.Property(p => p.ViewedAt)
          .IsRequired()
          .HasColumnType("timestamp without time zone");
});
        }
    }
}