using analytics_dashboard.models;
using analytics_dashboard.dtos.analytics;

namespace analytics_dashboard.interfaces
{
    public interface IPageviewService
    {
        Task<Pageview> AddPageview(Pageview newPageview);
        Task<List<Pageview>> GetPageviewsByArticleId(int articleId);
        Task<List<Pageview>> GetPageviewsByDateRange(DateTime startDate, DateTime endDate);
        Task<bool> DeletePageview(int id);
        
        Task<AnalyticsSummaryDto> GetAnalyticsSummary(AnalyticsFilterDto filter);
        Task<List<DailyViewsDto>> GetDailyViews(AnalyticsFilterDto filter);
        Task<List<TopArticleDto>> GetTopArticles(AnalyticsFilterDto filter);
        Task<List<Pageview>> GetRecentPageviews(AnalyticsFilterDto filter);
        
        Task<List<Pageview>> GetFilteredPageviews(AnalyticsFilterDto filter);
    }
}