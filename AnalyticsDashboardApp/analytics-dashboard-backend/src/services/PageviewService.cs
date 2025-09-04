using Microsoft.EntityFrameworkCore;
using analytics_dashboard.interfaces;
using analytics_dashboard.models;
using analytics_dashboard.data;
using analytics_dashboard.dtos.analytics;

namespace analytics_dashboard.services
{
    public class PageviewService : IPageviewService
    {
        private readonly AppDbContext _context;

        public PageviewService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Pageview> AddPageview(Pageview newPageview)
        {
            try
            {
                await _context.Pageviews.AddAsync(newPageview);
                await _context.SaveChangesAsync();
                return newPageview;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding pageview: {ex.Message}");
                throw new Exception("Failed to add pageview. Please try again.");
            }
        }

        public async Task<List<Pageview>> GetPageviewsByArticleId(int articleId)
        {
            try
            {
                return await _context.Pageviews
                    .Where(p => p.ArticleId == articleId)
                    .OrderByDescending(p => p.ViewedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting pageviews for article {articleId}: {ex.Message}");
                throw new Exception("Failed to retrieve pageviews. Please try again.");
            }
        }

        public async Task<List<Pageview>> GetPageviewsByDateRange(DateTime startDate, DateTime endDate)
        {
            try
            {
                return await _context.Pageviews
                    .Where(p => p.ViewedAt >= startDate && p.ViewedAt <= endDate)
                    .OrderByDescending(p => p.ViewedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting pageviews for date range {startDate} to {endDate}: {ex.Message}");
                throw new Exception("Failed to retrieve pageviews. Please try again.");
            }
        }

        public async Task<bool> DeletePageview(int id)
        {
            try
            {
                var pageview = await _context.Pageviews.FindAsync(id);
                if (pageview == null) 
                {
                    Console.WriteLine($"Pageview with ID {id} not found for deletion");
                    return false;
                }

                _context.Pageviews.Remove(pageview);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting pageview {id}: {ex.Message}");
                throw new Exception("Failed to delete pageview. Please try again.");
            }
        }

        public async Task<AnalyticsSummaryDto> GetAnalyticsSummary(AnalyticsFilterDto filter)
        {
            try
            {
                var pageviews = await GetFilteredPageviews(filter);
                
                if (!pageviews.Any())
                {
                    return new AnalyticsSummaryDto();
                }

                var totalViews = pageviews.Count;
                var averageTimeOnPage = pageviews.Average(p => p.DurationSeconds);
                var bounceRate = (double)pageviews.Count(p => p.IsBounce) / totalViews * 100;

                return new AnalyticsSummaryDto
                {
                    TotalViews = totalViews,
                    AverageTimeOnPage = averageTimeOnPage,
                    BounceRate = bounceRate
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting analytics summary: {ex.Message}");
                throw new Exception("Failed to retrieve analytics summary. Please try again.");
            }
        }

        public async Task<List<DailyViewsDto>> GetDailyViews(AnalyticsFilterDto filter)
        {
            try
            {
                var pageviews = await GetFilteredPageviews(filter);
                
                return pageviews
                    .GroupBy(p => p.ViewedAt.Date)
                    .Select(g => new DailyViewsDto
                    {
                        Date = g.Key,
                        Views = g.Count()
                    })
                    .OrderBy(d => d.Date)
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting daily views: {ex.Message}");
                throw new Exception("Failed to retrieve daily views. Please try again.");
            }
        }

        public async Task<List<TopArticleDto>> GetTopArticles(AnalyticsFilterDto filter)
        {
            try
            {
                var pageviews = await GetFilteredPageviews(filter);
                
                return pageviews
                    .GroupBy(p => p.ArticleId)
                    .Select(g => new TopArticleDto
                    {
                        ArticleId = g.Key,
                        Title = _context.Articles.FirstOrDefault(a => a.Id == g.Key)?.Title ?? "Unknown Article",
                        Views = g.Count(),
                        AverageTimeOnPage = g.Average(p => p.DurationSeconds),
                        BounceRate = g.Count() > 0 ? (double)g.Count(p => p.IsBounce) / g.Count() * 100 : 0
                    })
                    .OrderByDescending(a => a.Views)
                    .Take(10)
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting top articles: {ex.Message}");
                throw new Exception("Failed to retrieve top articles. Please try again.");
            }
        }

        public async Task<List<Pageview>> GetRecentPageviews(AnalyticsFilterDto filter)
        {
            try
            {
                var pageviews = await GetFilteredPageviews(filter);
                
                return pageviews
                    .OrderByDescending(p => p.ViewedAt)
                    .Take(50)
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting recent pageviews: {ex.Message}");
                throw new Exception("Failed to retrieve recent pageviews. Please try again.");
            }
        }

        public async Task<List<Pageview>> GetFilteredPageviews(AnalyticsFilterDto filter)
        {
            try
            {
                var query = _context.Pageviews
                    .Include(p => p.Article)
                    .AsQueryable();

                if (filter.StartDate.HasValue)
                {
                    var startDate = filter.StartDate.Value.ToUniversalTime();
                    Console.WriteLine($"Applying start date filter (UTC): {startDate:yyyy-MM-dd HH:mm:ss}");
                    query = query.Where(p => p.ViewedAt.ToUniversalTime() >= startDate);
                }

                if (filter.EndDate.HasValue)
                {
                    var endDate = filter.EndDate.Value.ToUniversalTime().AddDays(1).AddTicks(-1);
                    Console.WriteLine($"Applying end date filter (UTC): {endDate:yyyy-MM-dd HH:mm:ss}");
                    query = query.Where(p => p.ViewedAt.ToUniversalTime() <= endDate);
                }

                if (!string.IsNullOrEmpty(filter.Category) && filter.Category != "All Categories")
                {
                    Console.WriteLine($"Applying category filter: '{filter.Category}'");
                    query = query.Where(p => p.Article != null && p.Article.Category == filter.Category);
                }

                var results = await query.ToListAsync();
                Console.WriteLine($"Found {results.Count} pageviews after filtering");
                return results;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting filtered pageviews: {ex.Message}");
                throw new Exception("Failed to retrieve filtered pageviews. Please try again.");
            }
        }
    }
}