using analytics_dashboard.data;
using analytics_dashboard.models;
using Microsoft.EntityFrameworkCore;

namespace analytics_dashboard.services
{
    public class DataSeederService
    {
        private readonly AppDbContext _context;
        private readonly Random _random = new Random();

        public DataSeederService(AppDbContext context)
        {
            _context = context;
        }

        public async Task SeedDataAsync()
        {
            if (await _context.Articles.AnyAsync())
            {
                return;
            }

            var categories = new[] { "Technology", "Business", "Lifestyle" };

            // Create 20-30 articles
            var articles = new List<Article>();
            var articleCount = _random.Next(20, 31);

            for (int i = 1; i <= articleCount; i++)
            {
                var category = categories[_random.Next(categories.Length)];
                var article = new Article
                {
                    Title = $"{category} Article {i}",
                    Category = category,
                    PublishedAt = DateTime.Now.AddDays(-_random.Next(120, 180)), // 4-6 months old
                    ArticleDetail = new ArticleDetail
                    {
                        Summary = $"This is a summary for {category} Article {i} discussing important topics in this field.",
                        HeroImageUrl = $"https://example.com/images/{category.ToLower()}{i}.jpg",
                        ReadingTimeSeconds = _random.Next(180, 720) // 3-12 minutes
                    }
                };
                articles.Add(article);
            }

            await _context.Articles.AddRangeAsync(articles);
            await _context.SaveChangesAsync();

            // Create 30,000-60,000 pageviews
            var pageviews = new List<Pageview>();
            var pageviewCount = _random.Next(30000, 60001);
            var startDate = DateTime.Now.AddDays(-90); // Last 90 days

            for (int i = 0; i < pageviewCount; i++)
            {
                var article = articles[_random.Next(articles.Count)];
                var viewedAt = startDate.AddSeconds(_random.Next(0, 90 * 24 * 60 * 60)); // Random time in last 90 days
                
                var pageview = new Pageview
                {
                    ArticleId = article.Id,
                    ViewedAt = viewedAt,
                    DurationSeconds = _random.Next(5, 600), // 5 seconds to 10 minutes
                    IsBounce = _random.NextDouble() < 0.4 // 40% bounce rate
                };
                
                pageviews.Add(pageview);

                if (pageviews.Count % 1000 == 0)
                {
                    await _context.Pageviews.AddRangeAsync(pageviews);
                    await _context.SaveChangesAsync();
                    _context.ChangeTracker.Clear();
                    pageviews.Clear();
                }
            }

            if (pageviews.Any())
            {
                await _context.Pageviews.AddRangeAsync(pageviews);
                await _context.SaveChangesAsync();
            }
        }
    }
}