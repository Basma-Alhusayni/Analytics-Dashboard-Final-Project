using Microsoft.EntityFrameworkCore;
using analytics_dashboard.data;
using analytics_dashboard.interfaces;
using analytics_dashboard.models;
using analytics_dashboard.dtos.analytics;

namespace analytics_dashboard.services
{
    public class ArticleService : IArticleService
    {
        private readonly AppDbContext _context;

        public ArticleService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Article>> GetAllArticles()
        {
            return await _context.Articles
                .Include(a => a.ArticleDetail)
                .OrderByDescending(a => a.PublishedAt)
                .ToListAsync();
        }

        public async Task<Article?> GetArticleById(int id)
        {
            return await _context.Articles
                .Include(a => a.ArticleDetail)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<Article> AddArticle(Article newArticle)
        {
            await _context.Articles.AddAsync(newArticle);
            await _context.SaveChangesAsync();
            return newArticle;
        }

        public async Task<Article?> UpdateArticle(int id, Article updatedArticle)
        {
            var existingArticle = await _context.Articles
                .Include(a => a.ArticleDetail)
                .FirstOrDefaultAsync(a => a.Id == id);
                
            if (existingArticle == null) return null;

            existingArticle.Title = updatedArticle.Title;
            existingArticle.Category = updatedArticle.Category;
            existingArticle.PublishedAt = updatedArticle.PublishedAt;

            if (updatedArticle.ArticleDetail != null)
            {
                if (existingArticle.ArticleDetail == null)
                {
                    existingArticle.ArticleDetail = new ArticleDetail 
                    { 
                        ArticleId = id 
                    };
                    _context.ArticleDetails.Add(existingArticle.ArticleDetail);
                }

                existingArticle.ArticleDetail.Summary = updatedArticle.ArticleDetail.Summary;
                existingArticle.ArticleDetail.HeroImageUrl = updatedArticle.ArticleDetail.HeroImageUrl;
                existingArticle.ArticleDetail.ReadingTimeSeconds = updatedArticle.ArticleDetail.ReadingTimeSeconds;
            }

            await _context.SaveChangesAsync();
            return existingArticle;
        }

        public async Task<bool> DeleteArticle(int id)
        {
            var article = await _context.Articles
                .Include(a => a.ArticleDetail)
                .FirstOrDefaultAsync(a => a.Id == id);
                
            if (article == null) return false;

            _context.Articles.Remove(article);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Article>> SearchArticles(string searchTerm, string? category)
        {
            var query = _context.Articles
                .Include(a => a.ArticleDetail)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(a => a.Title.Contains(searchTerm));
            }

            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(a => a.Category == category);
            }

            return await query.OrderByDescending(a => a.PublishedAt).ToListAsync();
        }

          public async Task<List<Article>> GetFilteredArticles(AnalyticsFilterDto filter)
        {
            Console.WriteLine($"article filter debug start:");
            Console.WriteLine($"Received filter - Category: '{filter.Category}', StartDate: {filter.StartDate}, EndDate: {filter.EndDate}");

            var query = _context.Articles.Include(a => a.ArticleDetail).AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.Category) && filter.Category != "All Categories")
            {
                var categoryFilter = filter.Category.Trim().ToLower();
                Console.WriteLine($"Applying category filter: '{categoryFilter}'");
                query = query.Where(a => a.Category.ToLower() == categoryFilter);
            }

            if (filter.StartDate.HasValue)
            {
                var startDate = filter.StartDate.Value.Date;
                Console.WriteLine($"Applying start date filter: {startDate:yyyy-MM-dd}");
                query = query.Where(a => a.PublishedAt.Date >= startDate);
            }

            if (filter.EndDate.HasValue)
            {
                var endDate = filter.EndDate.Value.Date;
                Console.WriteLine($"Applying end date filter: {endDate:yyyy-MM-dd}");
                query = query.Where(a => a.PublishedAt.Date <= endDate);
            }

            var results = await query.OrderByDescending(a => a.PublishedAt).ToListAsync();
            
            Console.WriteLine($"Filtered results ({results.Count} articles):");
            foreach (var article in results)
            {
                Console.WriteLine($"  - '{article.Title}' (Category: '{article.Category}', Published: {article.PublishedAt:yyyy-MM-dd})");
            }
            Console.WriteLine($"end debug");
            
            return results;
        }
    }
}